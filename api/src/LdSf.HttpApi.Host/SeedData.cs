using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Guids;

namespace LdSf;

public static class SeedData
{
    public static async Task EnsureSeededAsync(System.IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LdSfDbContext>();
        await db.Database.EnsureCreatedAsync();

        if (await db.AdminUsers.AnyAsync())
        {
            return;
        }

        var guid = scope.ServiceProvider.GetRequiredService<IGuidGenerator>();
        var admin = new AdminUser(guid.Create(), "admin", "系统管理员", PasswordHasher.Hash("admin123"));
        var sub = new Subaccount(guid.Create(), "demo-subaccount", "示例分后台", 10, admin.Id);
        var code = new AuthorizationCode(guid.Create(), "DEMO-001", sub.Id, 10);
        var task = new SmsTask(guid.Create(), sub.Id, "示例通知任务", 2, 1, "这是一条示例通知，请仅发送给已授权接收人。", null);

        db.AdminUsers.Add(admin);
        db.Subaccounts.Add(sub);
        db.AuthorizationCodes.Add(code);
        db.SmsTasks.Add(task);
        db.TaskPhoneNumbers.AddRange(
            new TaskPhoneNumber(guid.Create(), task.Id, "13800000000"),
            new TaskPhoneNumber(guid.Create(), task.Id, "13900000000"));
        db.UsageLedgers.Add(new UsageLedger(guid.Create(), sub.Id, code.Identifier, UsageLedgerType.Grant, 10, null, "初始化示例"));
        await db.SaveChangesAsync();
    }
}
