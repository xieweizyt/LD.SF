using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace LdSf;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Starting LD.SF database migrator...");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplication<LdSfDbMigratorModule>();

        await using var serviceProvider = services.BuildServiceProvider();
        var application = serviceProvider.GetRequiredService<IAbpApplicationWithExternalServiceProvider>();
        await application.InitializeAsync(serviceProvider);
        await LdSfDatabaseMigrationService.MigrateAndSeedAsync(serviceProvider);
        await application.ShutdownAsync();

        Console.WriteLine("LD.SF database migration completed.");
        return 0;
    }
}
