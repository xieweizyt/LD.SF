using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LdSf.Migrations;

[DbContext(typeof(LdSfDbContext))]
partial class LdSfDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

        modelBuilder.Entity("LdSf.AdminUser", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<string>("UserName").IsRequired().HasMaxLength(64).HasColumnType("nvarchar(64)");
            b.Property<string>("DisplayName").IsRequired().HasMaxLength(64).HasColumnType("nvarchar(64)");
            b.Property<string>("PasswordHash").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            b.Property<string>("Role").IsRequired().HasMaxLength(32).HasColumnType("nvarchar(32)");
            b.Property<string>("ExtraProperties").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("ConcurrencyStamp").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            b.Property<DateTime>("CreationTime").HasColumnType("datetime2");
            b.Property<Guid?>("CreatorId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("LastModificationTime").HasColumnType("datetime2");
            b.Property<Guid?>("LastModifierId").HasColumnType("uniqueidentifier");
            b.Property<bool>("IsDeleted").HasColumnType("bit").HasDefaultValue(false);
            b.Property<Guid?>("DeleterId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("DeletionTime").HasColumnType("datetime2");
            b.HasKey("Id");
            b.HasIndex("UserName").IsUnique();
            b.ToTable("LdSfAdminUsers");
        });

        modelBuilder.Entity("LdSf.Subaccount", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<string>("PublicId").IsRequired().HasMaxLength(32).HasColumnType("nvarchar(32)");
            b.Property<string>("Name").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            b.Property<int>("Balance").HasColumnType("int");
            b.Property<int>("SentCount").HasColumnType("int");
            b.Property<Guid>("AdminUserId").HasColumnType("uniqueidentifier");
            b.Property<string>("ExtraProperties").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("ConcurrencyStamp").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            b.Property<DateTime>("CreationTime").HasColumnType("datetime2");
            b.Property<Guid?>("CreatorId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("LastModificationTime").HasColumnType("datetime2");
            b.Property<Guid?>("LastModifierId").HasColumnType("uniqueidentifier");
            b.Property<bool>("IsDeleted").HasColumnType("bit").HasDefaultValue(false);
            b.Property<Guid?>("DeleterId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("DeletionTime").HasColumnType("datetime2");
            b.HasKey("Id");
            b.HasIndex("PublicId").IsUnique();
            b.ToTable("LdSfSubaccounts");
        });

        modelBuilder.Entity("LdSf.AuthorizationCode", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<string>("Identifier").IsRequired().HasMaxLength(64).HasColumnType("nvarchar(64)");
            b.Property<Guid>("SubaccountId").HasColumnType("uniqueidentifier");
            b.Property<int>("TotalGranted").HasColumnType("int");
            b.Property<int>("RemainingUses").HasColumnType("int");
            b.Property<int>("UsedCount").HasColumnType("int");
            b.Property<bool>("Active").HasColumnType("bit");
            b.Property<string>("AppPublicKeyPem").HasColumnType("nvarchar(max)");
            b.Property<string>("ExtraProperties").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("ConcurrencyStamp").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            b.Property<DateTime>("CreationTime").HasColumnType("datetime2");
            b.Property<Guid?>("CreatorId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("LastModificationTime").HasColumnType("datetime2");
            b.Property<Guid?>("LastModifierId").HasColumnType("uniqueidentifier");
            b.Property<bool>("IsDeleted").HasColumnType("bit").HasDefaultValue(false);
            b.Property<Guid?>("DeleterId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("DeletionTime").HasColumnType("datetime2");
            b.HasKey("Id");
            b.HasIndex("Identifier").IsUnique();
            b.ToTable("LdSfAuthorizationCodes");
        });

        modelBuilder.Entity("LdSf.SmsTask", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<Guid>("SubaccountId").HasColumnType("uniqueidentifier");
            b.Property<string>("TaskName").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            b.Property<int>("TotalNumbers").HasColumnType("int");
            b.Property<int>("BatchSize").HasColumnType("int");
            b.Property<string>("Content1").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("Content2").HasColumnType("nvarchar(max)");
            b.Property<int>("SentCount").HasColumnType("int");
            b.Property<int>("Status").HasColumnType("int");
            b.Property<string>("ExtraProperties").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("ConcurrencyStamp").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            b.Property<DateTime>("CreationTime").HasColumnType("datetime2");
            b.Property<Guid?>("CreatorId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("LastModificationTime").HasColumnType("datetime2");
            b.Property<Guid?>("LastModifierId").HasColumnType("uniqueidentifier");
            b.Property<bool>("IsDeleted").HasColumnType("bit").HasDefaultValue(false);
            b.Property<Guid?>("DeleterId").HasColumnType("uniqueidentifier");
            b.Property<DateTime?>("DeletionTime").HasColumnType("datetime2");
            b.HasKey("Id");
            b.HasIndex("SubaccountId");
            b.ToTable("LdSfSmsTasks");
        });

        modelBuilder.Entity("LdSf.TaskPhoneNumber", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<Guid>("TaskId").HasColumnType("uniqueidentifier");
            b.Property<string>("PhoneNumber").IsRequired().HasMaxLength(32).HasColumnType("nvarchar(32)");
            b.Property<bool>("ConfirmedSent").HasColumnType("bit");
            b.Property<DateTime?>("ConfirmedAt").HasColumnType("datetime2");
            b.HasKey("Id");
            b.HasIndex("TaskId", "ConfirmedSent");
            b.ToTable("LdSfTaskPhoneNumbers");
        });

        modelBuilder.Entity("LdSf.SendRecord", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<Guid>("TaskId").HasColumnType("uniqueidentifier");
            b.Property<Guid>("SubaccountId").HasColumnType("uniqueidentifier");
            b.Property<string>("PhoneNumber").IsRequired().HasMaxLength(32).HasColumnType("nvarchar(32)");
            b.Property<string>("Content").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<int>("Status").HasColumnType("int");
            b.Property<string>("ExtraProperties").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("ConcurrencyStamp").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            b.Property<DateTime>("CreationTime").HasColumnType("datetime2");
            b.Property<Guid?>("CreatorId").HasColumnType("uniqueidentifier");
            b.HasKey("Id");
            b.HasIndex("TaskId");
            b.HasIndex("SubaccountId");
            b.ToTable("LdSfSendRecords");
        });

        modelBuilder.Entity("LdSf.UsageLedger", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<Guid>("SubaccountId").HasColumnType("uniqueidentifier");
            b.Property<string>("Identifier").IsRequired().HasMaxLength(64).HasColumnType("nvarchar(64)");
            b.Property<int>("Type").HasColumnType("int");
            b.Property<int>("Count").HasColumnType("int");
            b.Property<Guid?>("TaskId").HasColumnType("uniqueidentifier");
            b.Property<string>("Note").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            b.Property<string>("ExtraProperties").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("ConcurrencyStamp").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            b.Property<DateTime>("CreationTime").HasColumnType("datetime2");
            b.Property<Guid?>("CreatorId").HasColumnType("uniqueidentifier");
            b.HasKey("Id");
            b.HasIndex("Identifier");
            b.ToTable("LdSfUsageLedgers");
        });
    }
}
