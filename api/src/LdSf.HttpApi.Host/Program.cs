using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LdSf;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.AddAppSettingsSecretsJson();
        await builder.AddApplicationAsync<LdSfHttpApiHostModule>();
        var app = builder.Build();
        await app.InitializeApplicationAsync();
        await SeedData.EnsureSeededAsync(app.Services);
        await app.RunAsync();
        return 0;
    }
}
