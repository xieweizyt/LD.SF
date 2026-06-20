using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LdSf.Migrations;

[DbContext(typeof(LdSfDbContext))]
[Migration("20260620000100_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LdSfAdminUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Role = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LdSfAdminUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LdSfAuthorizationCodes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Identifier = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                SubaccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TotalGranted = table.Column<int>(type: "int", nullable: false),
                RemainingUses = table.Column<int>(type: "int", nullable: false),
                UsedCount = table.Column<int>(type: "int", nullable: false),
                Active = table.Column<bool>(type: "bit", nullable: false),
                AppPublicKeyPem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LdSfAuthorizationCodes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LdSfSendRecords",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SubaccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LdSfSendRecords", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LdSfSmsTasks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SubaccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TaskName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                TotalNumbers = table.Column<int>(type: "int", nullable: false),
                BatchSize = table.Column<int>(type: "int", nullable: false),
                Content1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Content2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SentCount = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LdSfSmsTasks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LdSfSubaccounts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PublicId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Balance = table.Column<int>(type: "int", nullable: false),
                SentCount = table.Column<int>(type: "int", nullable: false),
                AdminUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LdSfSubaccounts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LdSfTaskPhoneNumbers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                ConfirmedSent = table.Column<bool>(type: "bit", nullable: false),
                ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LdSfTaskPhoneNumbers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LdSfUsageLedgers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SubaccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Identifier = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Count = table.Column<int>(type: "int", nullable: false),
                TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Note = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LdSfUsageLedgers", x => x.Id);
            });

        migrationBuilder.CreateIndex("IX_LdSfAdminUsers_UserName", "LdSfAdminUsers", "UserName", unique: true);
        migrationBuilder.CreateIndex("IX_LdSfAuthorizationCodes_Identifier", "LdSfAuthorizationCodes", "Identifier", unique: true);
        migrationBuilder.CreateIndex("IX_LdSfSendRecords_SubaccountId", "LdSfSendRecords", "SubaccountId");
        migrationBuilder.CreateIndex("IX_LdSfSendRecords_TaskId", "LdSfSendRecords", "TaskId");
        migrationBuilder.CreateIndex("IX_LdSfSmsTasks_SubaccountId", "LdSfSmsTasks", "SubaccountId");
        migrationBuilder.CreateIndex("IX_LdSfSubaccounts_PublicId", "LdSfSubaccounts", "PublicId", unique: true);
        migrationBuilder.CreateIndex("IX_LdSfTaskPhoneNumbers_TaskId_ConfirmedSent", "LdSfTaskPhoneNumbers", new[] { "TaskId", "ConfirmedSent" });
        migrationBuilder.CreateIndex("IX_LdSfUsageLedgers_Identifier", "LdSfUsageLedgers", "Identifier");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("LdSfAdminUsers");
        migrationBuilder.DropTable("LdSfAuthorizationCodes");
        migrationBuilder.DropTable("LdSfSendRecords");
        migrationBuilder.DropTable("LdSfSmsTasks");
        migrationBuilder.DropTable("LdSfSubaccounts");
        migrationBuilder.DropTable("LdSfTaskPhoneNumbers");
        migrationBuilder.DropTable("LdSfUsageLedgers");
    }
}
