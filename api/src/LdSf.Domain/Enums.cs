namespace LdSf;

public enum SmsTaskStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public enum SendRecordStatus
{
    Success = 0,
    Failed = 1
}

public enum UsageLedgerType
{
    Grant = 0,
    Consume = 1
}

