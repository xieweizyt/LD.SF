using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LdSf;

public class AuthorizationCode : FullAuditedAggregateRoot<Guid>
{
    public string Identifier { get; private set; } = string.Empty;
    public Guid SubaccountId { get; private set; }
    public int TotalGranted { get; private set; }
    public int RemainingUses { get; private set; }
    public int UsedCount { get; private set; }
    public bool Active { get; private set; } = true;
    public string? AppPublicKeyPem { get; private set; }

    protected AuthorizationCode()
    {
    }

    public AuthorizationCode(Guid id, string identifier, Guid subaccountId, int initialUses) : base(id)
    {
        Identifier = identifier;
        SubaccountId = subaccountId;
        TotalGranted = initialUses;
        RemainingUses = initialUses;
    }

    public void BindPublicKey(string publicKeyPem)
    {
        AppPublicKeyPem = publicKeyPem;
    }

    public void Grant(int count)
    {
        TotalGranted += count;
        RemainingUses += count;
    }

    public void Consume(int count)
    {
        RemainingUses -= count;
        UsedCount += count;
    }
}

