using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace LdSf.Controllers;

[ApiController]
[IgnoreAntiforgeryToken]
[Route("api/ldsf")]
public class LdSfController : AbpControllerBase
{
    private readonly ILdSfAppService _service;

    public LdSfController(ILdSfAppService service)
    {
        _service = service;
    }

    [HttpPost("auth/login")]
    public Task<LoginDto> LoginAsync([FromBody] LoginInput input) => _service.LoginAsync(input);

    [HttpGet("admin/subaccounts")]
    public Task<List<SubaccountDto>> GetSubaccountsAsync() => _service.GetSubaccountsAsync();

    [HttpPost("admin/subaccounts")]
    public Task<SubaccountDto> CreateSubaccountAsync([FromBody] CreateSubaccountInput input) => _service.CreateSubaccountAsync(input);

    [HttpPost("admin/subaccounts/{subaccountId:guid}/balance")]
    public Task<SubaccountDto> UpdateBalanceAsync(Guid subaccountId, [FromBody] UpdateBalanceInput input) => _service.UpdateBalanceAsync(subaccountId, input);

    [HttpPost("admin/authorizations/{identifier}/grant")]
    public Task<SubaccountDto> GrantUsesAsync(string identifier, [FromBody] GrantUsesInput input) => _service.GrantUsesAsync(identifier, input);

    [HttpGet("admin/ledger")]
    public Task<List<UsageLedgerDto>> GetLedgersAsync([FromQuery] string? identifier = null) => _service.GetLedgersAsync(identifier);

    [HttpGet("subaccounts/{subaccountId:guid}/tasks")]
    public Task<List<SmsTaskDto>> GetTasksAsync(Guid subaccountId) => _service.GetTasksAsync(subaccountId);

    [HttpPost("subaccounts/{subaccountId:guid}/tasks")]
    public Task<SmsTaskDto> CreateTaskAsync(Guid subaccountId, [FromBody] CreateTaskInput input) => _service.CreateTaskAsync(subaccountId, input);

    [HttpDelete("subaccounts/{subaccountId:guid}/tasks/{taskId:guid}")]
    public Task DeleteTaskAsync(Guid subaccountId, Guid taskId) => _service.DeleteTaskAsync(subaccountId, taskId);

    [HttpPost("app/authorize")]
    public Task<AuthorizationDto> AuthorizeAsync([FromBody] AuthorizeInput input) => _service.AuthorizeAsync(input);

    [HttpGet("app/tasks/{identifier}")]
    public Task<List<AppTaskDto>> GetAppTasksAsync(string identifier) => _service.GetAppTasksAsync(identifier);

    [HttpPost("app/tasks/{taskId:guid}/confirm-sent")]
    public Task ConfirmSentAsync(Guid taskId, [FromBody] ConfirmSentInput input) => _service.ConfirmSentAsync(taskId, input);
}
