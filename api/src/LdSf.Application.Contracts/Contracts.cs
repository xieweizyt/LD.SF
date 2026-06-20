using System;
using System.Collections.Generic;

namespace LdSf;

public class LoginInput
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
public record LoginDto(Guid UserId, string UserName, string DisplayName, string Role);

public class CreateSubaccountInput
{
    public string? Name { get; set; }
    public int InitialBalance { get; set; }
    public string? Identifier { get; set; }
}

public class UpdateBalanceInput
{
    public int NewBalance { get; set; }
}

public class GrantUsesInput
{
    public int Uses { get; set; }
    public string? Reason { get; set; }
}

public record SubaccountDto(
    Guid Id,
    string PublicId,
    string Name,
    int Balance,
    int SentCount,
    string? AuthorizationIdentifier,
    int TotalGranted,
    int UsedCount,
    DateTime CreationTime);

public class CreateTaskInput
{
    public string? TaskName { get; set; }
    public List<string>? PhoneNumbers { get; set; }
    public int BatchSize { get; set; }
    public string? Content1 { get; set; }
    public string? Content2 { get; set; }
}

public record SmsTaskDto(
    Guid Id,
    Guid SubaccountId,
    string TaskName,
    int TotalNumbers,
    int BatchSize,
    string Content1,
    string? Content2,
    int SentCount,
    SmsTaskStatus Status,
    DateTime CreationTime);

public class AuthorizeInput
{
    public string? Identifier { get; set; }
    public string? AppPublicKeyPem { get; set; }
}
public record AuthorizationPayloadDto(string Identifier, Guid SubaccountId, int RemainingUses, int TotalGranted, int UsedCount, DateTime ServerTime);
public record EncryptedEnvelopeDto(string Algorithm, string CipherText);
public record AuthorizationDto(AuthorizationPayloadDto Plain, EncryptedEnvelopeDto? Encrypted);
public record AppTaskDto(Guid Id, string TaskName, int BatchSize, string Content1, string? Content2, SmsTaskStatus Status, List<string> PhoneNumbers);
public class ConfirmSentInput
{
    public List<string>? PhoneNumbers { get; set; }
    public string Content { get; set; } = string.Empty;
}
public record UsageLedgerDto(Guid Id, Guid SubaccountId, string Identifier, UsageLedgerType Type, int Count, Guid? TaskId, string Note, DateTime CreationTime);
