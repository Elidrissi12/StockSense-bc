using Microsoft.EntityFrameworkCore;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Enums;
using StockManagement.Infrastructure.DbContext;

namespace StockManagement.API;

public static class DbSeeder
{
    public static async Task SeedDemoAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Applique les migrations si la DB est disponible.
        await db.Database.MigrateAsync(ct);

        if (!await db.Categories.AnyAsync(ct))
        {
            db.Categories.AddRange(
                new Category { Id = Guid.NewGuid(), Name = "Matières premières" },
                new Category { Id = Guid.NewGuid(), Name = "Produits finis" }
            );
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Users.AnyAsync(ct))
        {
            // Mot de passe de démo (à changer en environnement réel).
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Nom = "Admin",
                Email = "admin@stocksense.local",
                Role = UserRole.Admin,
                PasswordHash = passwordHasher.HashPassword("Admin123!"),
                CreatedAtUtc = DateTime.UtcNow
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync(ct);
        }
    }
}

