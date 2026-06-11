using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Threddit.Application.Interfaces.Driven;

namespace Threddit.Infrastructure.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> SeedDatabaseAsync(this IHost host)
    {
        var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
        await seeder.SeedAsync();
        return host;
    }
}