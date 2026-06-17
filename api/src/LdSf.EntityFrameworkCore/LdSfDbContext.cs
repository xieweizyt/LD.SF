using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace LdSf;

[ConnectionStringName("Default")]
public class LdSfDbContext : AbpDbContext<LdSfDbContext>
{
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Subaccount> Subaccounts => Set<Subaccount>();
    public DbSet<AuthorizationCode> AuthorizationCodes => Set<AuthorizationCode>();
    public DbSet<SmsTask> SmsTasks => Set<SmsTask>();
    public DbSet<TaskPhoneNumber> TaskPhoneNumbers => Set<TaskPhoneNumber>();
    public DbSet<SendRecord> SendRecords => Set<SendRecord>();
    public DbSet<UsageLedger> UsageLedgers => Set<UsageLedger>();

    public LdSfDbContext(DbContextOptions<LdSfDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AdminUser>(b =>
        {
            b.ToTable("LdSfAdminUsers");
            b.HasKey(x => x.Id);
            b.Property(x => x.UserName).HasMaxLength(64).IsRequired();
            b.HasIndex(x => x.UserName).IsUnique();
            b.Property(x => x.DisplayName).HasMaxLength(64).IsRequired();
            b.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            b.Property(x => x.Role).HasMaxLength(32).IsRequired();
        });

        builder.Entity<Subaccount>(b =>
        {
            b.ToTable("LdSfSubaccounts");
            b.HasKey(x => x.Id);
            b.Property(x => x.PublicId).HasMaxLength(32).IsRequired();
            b.HasIndex(x => x.PublicId).IsUnique();
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        builder.Entity<AuthorizationCode>(b =>
        {
            b.ToTable("LdSfAuthorizationCodes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Identifier).HasMaxLength(64).IsRequired();
            b.HasIndex(x => x.Identifier).IsUnique();
            b.Property(x => x.AppPublicKeyPem).HasColumnType("nvarchar(max)");
        });

        builder.Entity<SmsTask>(b =>
        {
            b.ToTable("LdSfSmsTasks");
            b.HasKey(x => x.Id);
            b.Property(x => x.TaskName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Content1).HasColumnType("nvarchar(max)");
            b.Property(x => x.Content2).HasColumnType("nvarchar(max)");
            b.HasIndex(x => x.SubaccountId);
        });

        builder.Entity<TaskPhoneNumber>(b =>
        {
            b.ToTable("LdSfTaskPhoneNumbers");
            b.HasKey(x => x.Id);
            b.Property(x => x.PhoneNumber).HasMaxLength(32).IsRequired();
            b.HasIndex(x => new { x.TaskId, x.ConfirmedSent });
        });

        builder.Entity<SendRecord>(b =>
        {
            b.ToTable("LdSfSendRecords");
            b.HasKey(x => x.Id);
            b.Property(x => x.PhoneNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Content).HasColumnType("nvarchar(max)");
            b.HasIndex(x => x.TaskId);
            b.HasIndex(x => x.SubaccountId);
        });

        builder.Entity<UsageLedger>(b =>
        {
            b.ToTable("LdSfUsageLedgers");
            b.HasKey(x => x.Id);
            b.Property(x => x.Identifier).HasMaxLength(64).IsRequired();
            b.Property(x => x.Note).HasMaxLength(256);
            b.HasIndex(x => x.Identifier);
        });
    }
}

