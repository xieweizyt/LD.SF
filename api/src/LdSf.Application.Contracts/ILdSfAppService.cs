using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LdSf;

public interface ILdSfAppService : IApplicationService
{
    Task<LoginDto> LoginAsync(LoginInput input);
    Task<List<SubaccountDto>> GetSubaccountsAsync();
    Task<SubaccountDto> CreateSubaccountAsync(CreateSubaccountInput input);
    Task<SubaccountDto> UpdateBalanceAsync(Guid subaccountId, UpdateBalanceInput input);
    Task<SubaccountDto> GrantUsesAsync(string identifier, GrantUsesInput input);
    Task<List<UsageLedgerDto>> GetLedgersAsync(string? identifier = null);
    Task<List<SmsTaskDto>> GetTasksAsync(Guid subaccountId);
    Task<SmsTaskDto> CreateTaskAsync(Guid subaccountId, CreateTaskInput input);
    Task DeleteTaskAsync(Guid subaccountId, Guid taskId);
    Task<AuthorizationDto> AuthorizeAsync(AuthorizeInput input);
    Task<List<AppTaskDto>> GetAppTasksAsync(string identifier);
    Task ConfirmSentAsync(Guid taskId, ConfirmSentInput input);
}

