using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StockManagement.Infrastructure.DbContext;

public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
{
    public StockDbContext CreateDbContext(string[] args)
    {
        // Lecture du chemin de config côté projet API (Design-time).
        var basePath = Directory.GetCurrentDirectory();

        var connString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default") ??
            Environment.GetEnvironmentVariable("STOCKSENSE_CONNECTIONSTRING") ??
            "Server=localhost;Database=StockManagement;Trusted_Connection=True;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();
        optionsBuilder.UseSqlServer(connString);
        return new StockDbContext(optionsBuilder.Options);
    }
}

