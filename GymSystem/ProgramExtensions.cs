using GymSystem.DAL.DataSeed;
using GymSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.PL
{
        public static class ProgramExtensions
        {
            public static async Task MigrateAndSeedAsync(this WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GymAppContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
             

                var pending = await dbContext.Database.GetPendingMigrationsAsync();
                if (pending.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pending.Count());
                    await dbContext.Database.MigrateAsync();
                }

                var seedPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
                await GymDataSeeding.SeedAsync(dbContext, seedPath, logger);
                await IdentityDataSeeding.SeedAsync(roleManager, userManager, logger);
            }
        }
}
