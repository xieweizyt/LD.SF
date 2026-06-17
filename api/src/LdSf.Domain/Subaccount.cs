using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LdSf;

public class Subaccount : FullAuditedAggregateRoot<Guid>
{
    public string PublicId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Balance { get; private set; }
    public int SentCount { get; private set; }
    public Guid AdminUserId { get; private set; }

    protected Subaccount()
    {
    }

    public Subaccount(Guid id, string publicId, string name, int balance, Guid adminUserId) : base(id)
    {
        PublicId = publicId;
        Name = name;
        Balance = balance;
        AdminUserId = adminUserId;
    }

    public void SetBalance(int newBalance)
    {
        Balance = newBalance < 0 ? 0 : newBalance;
    }

    public void AddBalance(int uses)
    {
        Balance += uses;
    }

    public void Consume(int count)
    {
        Balance -= count;
        SentCount += count;
    }
}

