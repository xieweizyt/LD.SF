using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace LdSf;

public class LdSfAppService : ApplicationService, ILdSfAppService
{
    private readonly IRepository<AdminUser, Guid> _adminUsers;
    private readonly IRepository<Subaccount, Guid> _subaccounts;
    private readonly IRepository<AuthorizationCode, Guid> _authorizationCodes;
    private readonly IRepository<SmsTask, Guid> _tasks;
    private readonly IRepository<TaskPhoneNumber, Guid> _phoneNumbers;
    private readonly IRepository<SendRecord, Guid> _sendRecords;
    private readonly IRepository<UsageLedger, Guid> _ledgers;
    private readonly IGuidGenerator _guidGenerator;

    public LdSfAppService(
        IRepository<AdminUser, Guid> adminUsers,
        IRepository<Subaccount, Guid> subaccounts,
        IRepository<AuthorizationCode, Guid> authorizationCodes,
        IRepository<SmsTask, Guid> tasks,
        IRepository<TaskPhoneNumber, Guid> phoneNumbers,
        IRepository<SendRecord, Guid> sendRecords,
        IRepository<UsageLedger, Guid> ledgers,
        IGuidGenerator guidGenerator)
    {
        _adminUsers = adminUsers;
        _subaccounts = subaccounts;
        _authorizationCodes = authorizationCodes;
        _tasks = tasks;
        _phoneNumbers = phoneNumbers;
        _sendRecords = sendRecords;
        _ledgers = ledgers;
        _guidGenerator = guidGenerator;
    }

    public async Task<LoginDto> LoginAsync(LoginInput input)
    {
        var user = (await _adminUsers.GetQueryableAsync()).FirstOrDefault(x => x.UserName == input.UserName);
        if (user == null || !PasswordHasher.Verify(input.Password, user.PasswordHash))
        {
            throw new UserFriendlyException("用户名或密码错误");
        }

        return new LoginDto(user.Id, user.UserName, user.DisplayName, user.Role);
    }

    public async Task<List<SubaccountDto>> GetSubaccountsAsync()
    {
        var subs = (await _subaccounts.GetQueryableAsync()).OrderByDescending(x => x.CreationTime).ToList();
        var codes = (await _authorizationCodes.GetQueryableAsync()).ToList();
        return subs.Select(x =>
        {
            var code = codes.FirstOrDefault(c => c.SubaccountId == x.Id);
            return ToDto(x, code);
        }).ToList();
    }

    public async Task<SubaccountDto> CreateSubaccountAsync(CreateSubaccountInput input)
    {
        if (input.InitialBalance < 0) throw new UserFriendlyException("初始次数不能小于 0");

        var admin = (await _adminUsers.GetQueryableAsync()).OrderBy(x => x.CreationTime).First();
        var sub = new Subaccount(_guidGenerator.Create(), New32(), input.Name.Trim(), input.InitialBalance, admin.Id);
        var identifier = string.IsNullOrWhiteSpace(input.Identifier) ? $"LD-{Random.Shared.Next(100000, 999999)}" : input.Identifier.Trim();
        var code = new AuthorizationCode(_guidGenerator.Create(), identifier, sub.Id, input.InitialBalance);
        var ledger = new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Grant, input.InitialBalance, null, "创建分后台");

        await _subaccounts.InsertAsync(sub);
        await _authorizationCodes.InsertAsync(code);
        await _ledgers.InsertAsync(ledger);
        return ToDto(sub, code);
    }

    public async Task<SubaccountDto> UpdateBalanceAsync(Guid subaccountId, UpdateBalanceInput input)
    {
        var sub = await _subaccounts.GetAsync(subaccountId);
        var code = (await _authorizationCodes.GetQueryableAsync()).FirstOrDefault(x => x.SubaccountId == sub.Id);
        var delta = input.NewBalance - sub.Balance;
        sub.SetBalance(input.NewBalance);
        if (code != null)
        {
            if (delta > 0)
            {
                code.Grant(delta);
                await _ledgers.InsertAsync(new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Grant, delta, null, "平台更新余额"));
            }
        }
        return ToDto(sub, code);
    }

    public async Task<SubaccountDto> GrantUsesAsync(string identifier, GrantUsesInput input)
    {
        if (input.Uses <= 0) throw new UserFriendlyException("增加次数必须大于 0");
        var code = (await _authorizationCodes.GetQueryableAsync()).FirstOrDefault(x => x.Identifier == identifier)
            ?? throw new UserFriendlyException("授权标识不存在");
        var sub = await _subaccounts.GetAsync(code.SubaccountId);
        code.Grant(input.Uses);
        sub.AddBalance(input.Uses);
        await _ledgers.InsertAsync(new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Grant, input.Uses, null, input.Reason ?? "平台增加次数"));
        return ToDto(sub, code);
    }

    public async Task<List<UsageLedgerDto>> GetLedgersAsync(string? identifier = null)
    {
        var query = await _ledgers.GetQueryableAsync();
        if (!identifier.IsNullOrWhiteSpace()) query = query.Where(x => x.Identifier == identifier);
        return query.OrderByDescending(x => x.CreationTime).Take(200)
            .Select(x => new UsageLedgerDto(x.Id, x.SubaccountId, x.Identifier, x.Type, x.Count, x.TaskId, x.Note, x.CreationTime))
            .ToList();
    }

    public async Task<List<SmsTaskDto>> GetTasksAsync(Guid subaccountId)
    {
        return (await _tasks.GetQueryableAsync())
            .Where(x => x.SubaccountId == subaccountId)
            .OrderByDescending(x => x.CreationTime)
            .Select(ToTaskDto)
            .ToList();
    }

    public async Task<SmsTaskDto> CreateTaskAsync(Guid subaccountId, CreateTaskInput input)
    {
        var phones = input.PhoneNumbers.Select(x => x.Trim()).Where(x => !x.IsNullOrWhiteSpace()).Distinct().ToList();
        if (phones.Count == 0) throw new UserFriendlyException("手机号不能为空");

        var task = new SmsTask(_guidGenerator.Create(), subaccountId, input.TaskName.Trim(), phones.Count, Math.Clamp(input.BatchSize, 1, 200), input.Content1.Trim(), input.Content2?.Trim());
        await _tasks.InsertAsync(task);
        foreach (var phone in phones)
        {
            await _phoneNumbers.InsertAsync(new TaskPhoneNumber(_guidGenerator.Create(), task.Id, phone));
        }
        return ToTaskDto(task);
    }

    public async Task DeleteTaskAsync(Guid subaccountId, Guid taskId)
    {
        var task = await _tasks.GetAsync(taskId);
        if (task.SubaccountId != subaccountId) throw new UserFriendlyException("任务不属于当前分后台");
        await _tasks.DeleteAsync(task);
    }

    public async Task<AuthorizationDto> AuthorizeAsync(AuthorizeInput input)
    {
        var code = (await _authorizationCodes.GetQueryableAsync()).FirstOrDefault(x => x.Identifier == input.Identifier && x.Active)
            ?? throw new UserFriendlyException("授权标识不存在或已停用");
        if (!input.AppPublicKeyPem.IsNullOrWhiteSpace())
        {
            code.BindPublicKey(input.AppPublicKeyPem!);
        }

        var plain = new AuthorizationPayloadDto(code.Identifier, code.SubaccountId, code.RemainingUses, code.TotalGranted, code.UsedCount, Clock.Now);
        return new AuthorizationDto(plain, RsaEnvelopeService.TryEncrypt(code.AppPublicKeyPem, plain));
    }

    public async Task<List<AppTaskDto>> GetAppTasksAsync(string identifier)
    {
        var code = (await _authorizationCodes.GetQueryableAsync()).FirstOrDefault(x => x.Identifier == identifier && x.Active)
            ?? throw new UserFriendlyException("授权标识不存在或已停用");
        var tasks = (await _tasks.GetQueryableAsync())
            .Where(x => x.SubaccountId == code.SubaccountId && (x.Status == SmsTaskStatus.Pending || x.Status == SmsTaskStatus.Processing))
            .OrderBy(x => x.CreationTime)
            .ToList();
        var phones = (await _phoneNumbers.GetQueryableAsync()).Where(x => tasks.Select(t => t.Id).Contains(x.TaskId) && !x.ConfirmedSent).ToList();
        return tasks.Select(x => new AppTaskDto(x.Id, x.TaskName, x.BatchSize, x.Content1, x.Content2, x.Status, phones.Where(p => p.TaskId == x.Id).Select(p => p.PhoneNumber).ToList())).ToList();
    }

    [UnitOfWork]
    public async Task ConfirmSentAsync(Guid taskId, ConfirmSentInput input)
    {
        var task = await _tasks.GetAsync(taskId);
        var code = (await _authorizationCodes.GetQueryableAsync()).FirstOrDefault(x => x.SubaccountId == task.SubaccountId && x.Active)
            ?? throw new UserFriendlyException("授权标识不存在或已停用");
        var sub = await _subaccounts.GetAsync(task.SubaccountId);
        var phones = (await _phoneNumbers.GetQueryableAsync())
            .Where(x => x.TaskId == task.Id && !x.ConfirmedSent && input.PhoneNumbers.Contains(x.PhoneNumber))
            .Take(code.RemainingUses)
            .ToList();
        if (phones.Count == 0) throw new UserFriendlyException("没有可确认的号码或剩余次数不足");

        foreach (var phone in phones)
        {
            phone.Confirm();
            await _sendRecords.InsertAsync(new SendRecord(_guidGenerator.Create(), task.Id, sub.Id, phone.PhoneNumber, input.Content, SendRecordStatus.Success));
        }

        task.ConfirmSent(phones.Count);
        sub.Consume(phones.Count);
        code.Consume(phones.Count);
        await _ledgers.InsertAsync(new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Consume, phones.Count, task.Id, "App 用户确认发送"));
    }

    private static SubaccountDto ToDto(Subaccount sub, AuthorizationCode? code) =>
        new(sub.Id, sub.PublicId, sub.Name, sub.Balance, sub.SentCount, code?.Identifier, code?.TotalGranted ?? 0, code?.UsedCount ?? 0, sub.CreationTime);

    private static SmsTaskDto ToTaskDto(SmsTask x) =>
        new(x.Id, x.SubaccountId, x.TaskName, x.TotalNumbers, x.BatchSize, x.Content1, x.Content2, x.SentCount, x.Status, x.CreationTime);

    private static string New32() => Convert.ToHexString(Guid.NewGuid().ToByteArray()).Replace("-", "").ToLowerInvariant()[..32];
}

