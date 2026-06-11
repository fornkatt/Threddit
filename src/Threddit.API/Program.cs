using System.Text;
using System.Threading.RateLimiting;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using Threddit.API.Extensions;
using Threddit.Application.Extensions;
using Threddit.Domain.Entities;
using Threddit.Infrastructure.Extensions;
using Threddit.Infrastructure.Persistence;

namespace Threddit.API;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        builder.Host.UseSerilog();

        if (!builder.Environment.IsDevelopment())
        {
            var keyVaultUri = new Uri("https://threddit-kv.vault.azure.net/");
            builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
        }

        builder.Services
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddApiServices();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowPresentation", policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.WithOrigins("https://localhost:7227", "http://localhost:5214",
                            "https://localhost:7246", "http://localhost:5036")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    policy.WithOrigins("https://lemon-forest-0a5afcf03.7.azurestaticapps.net")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });

        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ThredditDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenHandlers.Clear();
                options.TokenHandlers.Add(new JsonWebTokenHandler());

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("SiteOwnerOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("role", "SiteOwner");
            })
            .AddPolicy("SiteAdminOrOwner", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("role", "SiteAdmin", "SiteOwner");
            })
            .AddPolicy("NotBanned", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(ctx =>
                    !ctx.User.HasClaim("banned", "true"));
            });

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("registration", opt =>
            {
                opt.PermitLimit = 15;
                opt.Window = TimeSpan.FromHours(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
            options.AddFixedWindowLimiter("login", opt =>
            {
                opt.PermitLimit = 15;
                opt.Window = TimeSpan.FromMinutes(15);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
            options.AddFixedWindowLimiter("group-conversation", opt =>
            {
                opt.PermitLimit = 15;
                opt.Window = TimeSpan.FromDays(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            await app.SeedDatabaseAsync();
        }

        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseCors("AllowPresentation");
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.MapControllers();

        await app.RunAsync();
    }
}