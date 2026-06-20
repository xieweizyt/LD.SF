using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Guids;

namespace LdSf;

public static class LdSfDatabaseMigrationService
{
    public static async Task MigrateAndSeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("Default");

        await EnsureDatabaseCreatedAsync(connectionString);

        var db = scope.ServiceProvider.GetRequiredService<LdSfDbContext>();
        var dbConnection = db.Database.GetDbConnection();
        Console.WriteLine($"Migrating database: {dbConnection.DataSource}/{dbConnection.Database}");
        await db.Database.MigrateAsync();

        if (!await TableExistsAsync(connectionString, "LdSfAdminUsers"))
        {
            if (await TryRepairEmptyBrokenMigrationHistoryAsync(connectionString))
            {
                Console.WriteLine("Repaired broken migration history. Migrating database again...");
                await db.Database.MigrateAsync();
            }

            if (!await TableExistsAsync(connectionString, "LdSfAdminUsers"))
            {
                throw new InvalidOperationException(
                    "Database migration did not create table [LdSfAdminUsers]. " +
                    "The target database has a partial or inconsistent schema. " +
                    "For a development database, delete/drop the LDSF database and run LdSf.DbMigrator again.");
            }
        }

        var guid = scope.ServiceProvider.GetRequiredService<IGuidGenerator>();
        await EnsureSeedRowsAsync(connectionString, guid);
    }

    private static async Task EnsureDatabaseCreatedAsync(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return;
        }

        builder.InitialCatalog = "master";
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
IF DB_ID(N'{databaseName.Replace("'", "''")}') IS NULL
BEGIN
    CREATE DATABASE [{databaseName.Replace("]", "]]")}]
END
""";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<bool> TableExistsAsync(string? connectionString, string tableName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT CASE WHEN OBJECT_ID(@TableName, 'U') IS NULL THEN 0 ELSE 1 END";
        command.Parameters.AddWithValue("@TableName", $"dbo.{tableName}");
        return Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
    }

    private static async Task<bool> TryRepairEmptyBrokenMigrationHistoryAsync(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using (var ldTableCommand = connection.CreateCommand())
        {
            ldTableCommand.CommandText = """
SELECT COUNT(1)
FROM sys.tables
WHERE [name] LIKE N'LdSf%'
""";
            var ldTableCount = Convert.ToInt32(await ldTableCommand.ExecuteScalarAsync());
            if (ldTableCount > 0)
            {
                return false;
            }
        }

        await using (var historyCommand = connection.CreateCommand())
        {
            historyCommand.CommandText = "SELECT CASE WHEN OBJECT_ID(N'dbo.__EFMigrationsHistory', 'U') IS NULL THEN 0 ELSE 1 END";
            var hasHistoryTable = Convert.ToInt32(await historyCommand.ExecuteScalarAsync()) == 1;
            if (!hasHistoryTable)
            {
                return false;
            }
        }

        await using var repairCommand = connection.CreateCommand();
        repairCommand.CommandText = "DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260620000100_InitialCreate'";
        var affectedRows = await repairCommand.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

    private static async Task EnsureSeedRowsAsync(string? connectionString, IGuidGenerator guid)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using (var existsCommand = connection.CreateCommand())
        {
            existsCommand.CommandText = "SELECT COUNT(1) FROM [LdSfAdminUsers] WHERE [UserName] = N'admin'";
            var exists = Convert.ToInt32(await existsCommand.ExecuteScalarAsync()) > 0;
            if (exists)
            {
                await using var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE [LdSfAdminUsers] SET [PasswordHash] = @PasswordHash WHERE [UserName] = N'admin'";
                updateCommand.Parameters.AddWithValue("@PasswordHash", PasswordHasher.Hash("admin123"));
                await updateCommand.ExecuteNonQueryAsync();
                return;
            }
        }

        var adminId = guid.Create();
        var subaccountId = guid.Create();
        var authorizationId = guid.Create();
        var taskId = guid.Create();
        var ledgerId = guid.Create();
        var now = DateTime.UtcNow;

        await using var tx = await connection.BeginTransactionAsync();
        await ExecuteAsync(connection, tx, """
INSERT INTO [LdSfAdminUsers]
([Id], [UserName], [DisplayName], [PasswordHash], [Role], [ExtraProperties], [ConcurrencyStamp], [CreationTime], [IsDeleted])
VALUES
(@AdminId, N'admin', N'System Administrator', @PasswordHash, N'Admin', N'{}', @AdminStamp, @Now, CAST(0 AS bit));
""",
            ("@AdminId", adminId),
            ("@PasswordHash", PasswordHasher.Hash("admin123")),
            ("@AdminStamp", guid.Create().ToString("N")),
            ("@Now", now));

        await ExecuteAsync(connection, tx, """
INSERT INTO [LdSfSubaccounts]
([Id], [PublicId], [Name], [Balance], [SentCount], [AdminUserId], [ExtraProperties], [ConcurrencyStamp], [CreationTime], [IsDeleted])
VALUES
(@SubaccountId, N'demo-subaccount', N'Demo Subaccount', 10, 0, @AdminId, N'{}', @SubaccountStamp, @Now, CAST(0 AS bit));
""",
            ("@SubaccountId", subaccountId),
            ("@AdminId", adminId),
            ("@SubaccountStamp", guid.Create().ToString("N")),
            ("@Now", now));

        await ExecuteAsync(connection, tx, """
INSERT INTO [LdSfAuthorizationCodes]
([Id], [Identifier], [SubaccountId], [TotalGranted], [RemainingUses], [UsedCount], [Active], [ExtraProperties], [ConcurrencyStamp], [CreationTime], [IsDeleted])
VALUES
(@AuthorizationId, N'DEMO-001', @SubaccountId, 10, 10, 0, CAST(1 AS bit), N'{}', @AuthorizationStamp, @Now, CAST(0 AS bit));
""",
            ("@AuthorizationId", authorizationId),
            ("@SubaccountId", subaccountId),
            ("@AuthorizationStamp", guid.Create().ToString("N")),
            ("@Now", now));

        await ExecuteAsync(connection, tx, """
INSERT INTO [LdSfSmsTasks]
([Id], [SubaccountId], [TaskName], [TotalNumbers], [BatchSize], [Content1], [Content2], [SentCount], [Status], [ExtraProperties], [ConcurrencyStamp], [CreationTime], [IsDeleted])
VALUES
(@TaskId, @SubaccountId, N'Demo SMS Task', 2, 1, N'This is a demo SMS message. Send only to authorized recipients.', NULL, 0, 0, N'{}', @TaskStamp, @Now, CAST(0 AS bit));
""",
            ("@TaskId", taskId),
            ("@SubaccountId", subaccountId),
            ("@TaskStamp", guid.Create().ToString("N")),
            ("@Now", now));

        await ExecuteAsync(connection, tx, """
INSERT INTO [LdSfTaskPhoneNumbers]
([Id], [TaskId], [PhoneNumber], [ConfirmedSent])
VALUES
(@PhoneId1, @TaskId, N'13800000000', CAST(0 AS bit)),
(@PhoneId2, @TaskId, N'13900000000', CAST(0 AS bit));
""",
            ("@PhoneId1", guid.Create()),
            ("@PhoneId2", guid.Create()),
            ("@TaskId", taskId));

        await ExecuteAsync(connection, tx, """
INSERT INTO [LdSfUsageLedgers]
([Id], [SubaccountId], [Identifier], [Type], [Count], [TaskId], [Note], [ExtraProperties], [ConcurrencyStamp], [CreationTime])
VALUES
(@LedgerId, @SubaccountId, N'DEMO-001', 0, 10, NULL, N'Initial demo grant', N'{}', @LedgerStamp, @Now);
""",
            ("@LedgerId", ledgerId),
            ("@SubaccountId", subaccountId),
            ("@LedgerStamp", guid.Create().ToString("N")),
            ("@Now", now));

        await tx.CommitAsync();
    }

    private static async Task ExecuteAsync(SqlConnection connection, DbTransaction transaction, string sql, params (string Name, object? Value)[] parameters)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = (SqlTransaction)transaction;
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Name, parameter.Value ?? DBNull.Value);
        }

        await command.ExecuteNonQueryAsync();
    }
}
