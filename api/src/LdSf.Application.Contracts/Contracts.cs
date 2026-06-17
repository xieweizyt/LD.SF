using System;
using System.Collections.Generic;

namespace LdSf;

public record LoginInput(string UserName, string Password);
public record LoginDto(Guid UserId, string UserName, string DisplayName, string Role);

public record CreateSubaccountInput(string Name, int InitialBalance, string? Identifier);
public record UpdateBalanceInput(int NewBalance);
public record GrantUsesInput(int Uses, string? Reason);

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

public record CreateTaskInput(
    string TaskName,
    List<string> PhoneNumbers,
    int BatchSize,
    string Content1,
    string? Content2);

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

public record AuthorizeInput(string Identifier, string? AppPublicKeyPem);
public record AuthorizationPayloadDto(string Identifier, Guid SubaccountId, int RemainingUses, int TotalGranted, int UsedCount, DateTime ServerTime);
public record EncryptedEnvelopeDto(string Algorithm, string CipherText);
public record AuthorizationDto(AuthorizationPayloadDto Plain, EncryptedEnvelopeDto? Encrypted);
public record AppTaskDto(Guid Id, string TaskName, int BatchSize, string Content1, string? Content2, SmsTaskStatus Status, List<string> PhoneNumbers);
public record ConfirmSentInput(List<string> PhoneNumbers, string Content);
public record UsageLedgerDto(Guid Id, Guid SubaccountId, string Identifier, UsageLedgerType Type, int Count, Guid? TaskId, string Note, DateTime CreationTime);

