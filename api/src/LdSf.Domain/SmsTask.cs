using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LdSf;

public class SmsTask : FullAuditedAggregateRoot<Guid>
{
    public Guid SubaccountId { get; private set; }
    public string TaskName { get; private set; } = string.Empty;
    public int TotalNumbers { get; private set; }
    public int BatchSize { get; private set; }
    public string Content1 { get; private set; } = string.Empty;
    public string? Content2 { get; private set; }
    public int SentCount { get; private set; }
    public SmsTaskStatus Status { get; private set; } = SmsTaskStatus.Pending;

    protected SmsTask()
    {
    }

    public SmsTask(Guid id, Guid subaccountId, string taskName, int totalNumbers, int batchSize, string content1, string? content2) : base(id)
    {
        SubaccountId = subaccountId;
        TaskName = taskName;
        TotalNumbers = totalNumbers;
        BatchSize = batchSize;
        Content1 = content1;
        Content2 = content2;
    }

    public void ConfirmSent(int count)
    {
        SentCount += count;
        Status = SentCount >= TotalNumbers ? SmsTaskStatus.Completed : SmsTaskStatus.Processing;
    }
}

