namespace Threddit.Application.Interfaces.Driven;

public interface IDbSeeder
{
    /// <summary>Database seeder with certain initial values.</summary>
    Task SeedAsync();
}