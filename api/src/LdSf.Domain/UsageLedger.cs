using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LdSf;

public class UsageLedger : CreationAuditedAggregateRoot<Guid>
{
    public Guid SubaccountId { get; private set; }
    public string Identifier { get; private set; } = string.Empty;
    public UsageLedgerType Type { get; private set; }
    public int Count { get; private set; }
    public Guid? TaskId { get; private set; }
    public string Note { get; private set; } = string.Empty;

    protected UsageLedger()
    {
    }

    public UsageLedger(Guid id, Guid subaccountId, string identifier, UsageLedgerType type, int count, Guid? taskId, string note) : base(id)
    {
        SubaccountId = subaccountId;
        Identifier = identifier;
        Type = type;
        Count = count;
        TaskId = taskId;
        Note = note;
    }
}

