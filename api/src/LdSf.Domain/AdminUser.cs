using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LdSf;

public class AdminUser : FullAuditedAggregateRoot<Guid>
{
    public string UserName { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "Admin";

    protected AdminUser()
    {
    }

    public AdminUser(Guid id, string userName, string displayName, string passwordHash) : base(id)
    {
        UserName = userName;
        DisplayName = displayName;
        PasswordHash = passwordHash;
    }
}

