using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace LdSf;

public class LdSfAppService : ILdSfAppService, ITransientDependency
{
    private readonly LdSfDbContext _db;
    private readonly IGuidGenerator _guidGenerator;

    public LdSfAppService(LdSfDbContext db, IGuidGenerator guidGenerator)
    {
        _db = db;
        _guidGenerator = guidGenerator;
    }

    public Task<LoginDto> LoginAsync(LoginInput input)
    {
        var userName = input.UserName?.Trim() ?? string.Empty;
        var password = input.Password ?? string.Empty;
        var user = _db.AdminUsers.FirstOrDefault(x => x.UserName == userName);
        if (user == null || !PasswordHasher.Verify(password, user.PasswordHash))
        {
            throw new UserFriendlyException("Invalid user name or password");
        }

        return Task.FromResult(new LoginDto(user.Id, user.UserName, user.DisplayName, user.Role));
    }

    public Task<List<SubaccountDto>> GetSubaccountsAsync()
    {
        var subs = _db.Subaccounts.OrderByDescending(x => x.CreationTime).ToList();
        var codes = _db.AuthorizationCodes.ToList();
        var result = subs.Select(x =>
        {
            var code = codes.FirstOrDefault(c => c.SubaccountId == x.Id);
            return ToDto(x, code);
        }).ToList();
        return Task.FromResult(result);
    }

    public async Task<SubaccountDto> CreateSubaccountAsync(CreateSubaccountInput input)
    {
        if (input.InitialBalance < 0)
        {
            throw new UserFriendlyException("Initial balance cannot be negative");
        }

        var name = input.Name?.Trim();
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Subaccount name is required");
        }

        var admin = _db.AdminUsers.OrderBy(x => x.CreationTime).FirstOrDefault()
            ?? throw new UserFriendlyException("Admin user is not initialized. Run the database migrator first");
        var identifier = input.Identifier.IsNullOrWhiteSpace()
            ? $"LD-{Random.Shared.Next(100000, 999999)}"
            : input.Identifier!.Trim();

        if (_db.AuthorizationCodes.Any(x => x.Identifier == identifier))
        {
            throw new UserFriendlyException("Authorization identifier already exists");
        }

        var sub = new Subaccount(_guidGenerator.Create(), New32(), name!, input.InitialBalance, admin.Id);
        var code = new AuthorizationCode(_guidGenerator.Create(), identifier, sub.Id, input.InitialBalance);
        var ledger = new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Grant, input.InitialBalance, null, "Create subaccount");

        _db.Subaccounts.Add(sub);
        _db.AuthorizationCodes.Add(code);
        _db.UsageLedgers.Add(ledger);
        await _db.SaveChangesAsync();
        return ToDto(sub, code);
    }

    public async Task<SubaccountDto> UpdateBalanceAsync(Guid subaccountId, UpdateBalanceInput input)
    {
        var sub = _db.Subaccounts.FirstOrDefault(x => x.Id == subaccountId)
            ?? throw new UserFriendlyException("Subaccount does not exist");
        var code = _db.AuthorizationCodes.FirstOrDefault(x => x.SubaccountId == sub.Id);
        var delta = input.NewBalance - sub.Balance;
        sub.SetBalance(input.NewBalance);
        if (code != null && delta > 0)
        {
            code.Grant(delta);
            _db.UsageLedgers.Add(new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Grant, delta, null, "Platform balance update"));
        }

        await _db.SaveChangesAsync();
        return ToDto(sub, code);
    }

    public async Task<SubaccountDto> GrantUsesAsync(string identifier, GrantUsesInput input)
    {
        if (input.Uses <= 0)
        {
            throw new UserFriendlyException("Granted uses must be greater than 0");
        }

        var code = _db.AuthorizationCodes.FirstOrDefault(x => x.Identifier == identifier)
            ?? throw new UserFriendlyException("Authorization identifier does not exist");
        var sub = _db.Subaccounts.FirstOrDefault(x => x.Id == code.SubaccountId)
            ?? throw new UserFriendlyException("Subaccount does not exist");
        code.Grant(input.Uses);
        sub.AddBalance(input.Uses);
        _db.UsageLedgers.Add(new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Grant, input.Uses, null, input.Reason ?? "Platform grant"));
        await _db.SaveChangesAsync();
        return ToDto(sub, code);
    }

    public Task<List<UsageLedgerDto>> GetLedgersAsync(string? identifier = null)
    {
        var query = _db.UsageLedgers.AsQueryable();
        if (!identifier.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.Identifier == identifier);
        }

        var result = query.OrderByDescending(x => x.CreationTime).Take(200)
            .Select(x => new UsageLedgerDto(x.Id, x.SubaccountId, x.Identifier, x.Type, x.Count, x.TaskId, x.Note, x.CreationTime))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<SmsTaskDto>> GetTasksAsync(Guid subaccountId)
    {
        var result = _db.SmsTasks
            .Where(x => x.SubaccountId == subaccountId)
            .OrderByDescending(x => x.CreationTime)
            .Select(x => ToTaskDto(x))
            .ToList();
        return Task.FromResult(result);
    }

    public async Task<SmsTaskDto> CreateTaskAsync(Guid subaccountId, CreateTaskInput input)
    {
        if (!_db.Subaccounts.Any(x => x.Id == subaccountId))
        {
            throw new UserFriendlyException("Subaccount does not exist");
        }

        var phones = (input.PhoneNumbers ?? [])
            .Select(x => x.Trim())
            .Where(x => !x.IsNullOrWhiteSpace())
            .Distinct()
            .ToList();
        if (phones.Count == 0)
        {
            throw new UserFriendlyException("Phone number list is required");
        }

        var taskName = input.TaskName?.Trim();
        var content1 = input.Content1?.Trim();
        if (taskName.IsNullOrWhiteSpace() || content1.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Task name and SMS content are required");
        }

        var task = new SmsTask(_guidGenerator.Create(), subaccountId, taskName!, phones.Count, Math.Clamp(input.BatchSize, 1, 200), content1!, input.Content2?.Trim());
        _db.SmsTasks.Add(task);
        foreach (var phone in phones)
        {
            _db.TaskPhoneNumbers.Add(new TaskPhoneNumber(_guidGenerator.Create(), task.Id, phone));
        }

        await _db.SaveChangesAsync();
        return ToTaskDto(task);
    }

    public async Task DeleteTaskAsync(Guid subaccountId, Guid taskId)
    {
        var task = _db.SmsTasks.FirstOrDefault(x => x.Id == taskId)
            ?? throw new UserFriendlyException("Task does not exist");
        if (task.SubaccountId != subaccountId)
        {
            throw new UserFriendlyException("Task does not belong to the current subaccount");
        }

        _db.SmsTasks.Remove(task);
        await _db.SaveChangesAsync();
    }

    public async Task<AuthorizationDto> AuthorizeAsync(AuthorizeInput input)
    {
        var identifier = input.Identifier?.Trim() ?? string.Empty;
        var code = _db.AuthorizationCodes.FirstOrDefault(x => x.Identifier == identifier && x.Active)
            ?? throw new UserFriendlyException("Authorization identifier does not exist or is disabled");
        if (!input.AppPublicKeyPem.IsNullOrWhiteSpace())
        {
            code.BindPublicKey(input.AppPublicKeyPem!);
            await _db.SaveChangesAsync();
        }

        var plain = new AuthorizationPayloadDto(code.Identifier, code.SubaccountId, code.RemainingUses, code.TotalGranted, code.UsedCount, DateTime.UtcNow);
        return new AuthorizationDto(plain, RsaEnvelopeService.TryEncrypt(code.AppPublicKeyPem, plain));
    }

    public Task<List<AppTaskDto>> GetAppTasksAsync(string identifier)
    {
        var code = _db.AuthorizationCodes.FirstOrDefault(x => x.Identifier == identifier && x.Active)
            ?? throw new UserFriendlyException("Authorization identifier does not exist or is disabled");
        var tasks = _db.SmsTasks
            .Where(x => x.SubaccountId == code.SubaccountId && (x.Status == SmsTaskStatus.Pending || x.Status == SmsTaskStatus.Processing))
            .OrderBy(x => x.CreationTime)
            .ToList();
        var taskIds = tasks.Select(t => t.Id).ToList();
        var phones = _db.TaskPhoneNumbers
            .Where(x => taskIds.Contains(x.TaskId) && !x.ConfirmedSent)
            .ToList();
        var result = tasks.Select(x => new AppTaskDto(x.Id, x.TaskName, x.BatchSize, x.Content1, x.Content2, x.Status, phones.Where(p => p.TaskId == x.Id).Select(p => p.PhoneNumber).ToList())).ToList();
        return Task.FromResult(result);
    }

    public async Task ConfirmSentAsync(Guid taskId, ConfirmSentInput input)
    {
        var task = _db.SmsTasks.FirstOrDefault(x => x.Id == taskId)
            ?? throw new UserFriendlyException("Task does not exist");
        var code = _db.AuthorizationCodes.FirstOrDefault(x => x.SubaccountId == task.SubaccountId && x.Active)
            ?? throw new UserFriendlyException("Authorization identifier does not exist or is disabled");
        var sub = _db.Subaccounts.FirstOrDefault(x => x.Id == task.SubaccountId)
            ?? throw new UserFriendlyException("Subaccount does not exist");
        var inputPhones = input.PhoneNumbers ?? [];
        var phones = _db.TaskPhoneNumbers
            .Where(x => x.TaskId == task.Id && !x.ConfirmedSent && inputPhones.Contains(x.PhoneNumber))
            .Take(code.RemainingUses)
            .ToList();
        if (phones.Count == 0)
        {
            throw new UserFriendlyException("No confirmable phone numbers or no remaining uses");
        }

        foreach (var phone in phones)
        {
            phone.Confirm();
            _db.SendRecords.Add(new SendRecord(_guidGenerator.Create(), task.Id, sub.Id, phone.PhoneNumber, input.Content, SendRecordStatus.Success));
        }

        task.ConfirmSent(phones.Count);
        sub.Consume(phones.Count);
        code.Consume(phones.Count);
        _db.UsageLedgers.Add(new UsageLedger(_guidGenerator.Create(), sub.Id, code.Identifier, UsageLedgerType.Consume, phones.Count, task.Id, "App confirm sent"));
        await _db.SaveChangesAsync();
    }

    private static SubaccountDto ToDto(Subaccount sub, AuthorizationCode? code) =>
        new(sub.Id, sub.PublicId, sub.Name, sub.Balance, sub.SentCount, code?.Identifier, code?.TotalGranted ?? 0, code?.UsedCount ?? 0, sub.CreationTime);

    private static SmsTaskDto ToTaskDto(SmsTask x) =>
        new(x.Id, x.SubaccountId, x.TaskName, x.TotalNumbers, x.BatchSize, x.Content1, x.Content2, x.SentCount, x.Status, x.CreationTime);

    private static string New32() => Convert.ToHexString(Guid.NewGuid().ToByteArray()).Replace("-", "").ToLowerInvariant()[..32];
}
