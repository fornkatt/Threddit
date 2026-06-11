using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence;

public class DbSeeder : IDbSeeder
{
    private readonly ThredditDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public DbSeeder(
        IUserRepository userRepository,
        ThredditDbContext context,
        IConfiguration configuration
    )
    {
        _userRepository = userRepository;
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        // Seed site owner
        var ownerUsername = _configuration["SeedData:SiteOwnerUsername"];

        if (!string.IsNullOrWhiteSpace(ownerUsername))
        {
            var ownerToSeed = await _userRepository.GetByUsernameAsync(ownerUsername);

            if (ownerToSeed.IsSuccess)
            {
                var ownerId = ownerToSeed.Value!.Id;

                if (!await _context.SiteOwner.AnyAsync(so => so.UserId == ownerId))
                {
                    var newOwner = SiteOwner.Assign(ownerToSeed.Value!);
                    _context.SiteOwner.Add(newOwner);
                }

                if (!await _context.SiteAdmins.AnyAsync(sa => sa.UserId == ownerId))
                {
                    var newAdmin = SiteAdmin.Assign(ownerToSeed.Value!);
                    _context.SiteAdmins.Add(newAdmin);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}