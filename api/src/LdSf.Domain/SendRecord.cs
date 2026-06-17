using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LdSf;

public class SendRecord : CreationAuditedAggregateRoot<Guid>
{
    public Guid TaskId { get; private set; }
    public Guid SubaccountId { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public SendRecordStatus Status { get; private set; }

    protected SendRecord()
    {
    }

    public SendRecord(Guid id, Guid taskId, Guid subaccountId, string phoneNumber, string content, SendRecordStatus status) : base(id)
    {
        TaskId = taskId;
        SubaccountId = subaccountId;
        PhoneNumber = phoneNumber;
        Content = content;
        Status = status;
    }
}

