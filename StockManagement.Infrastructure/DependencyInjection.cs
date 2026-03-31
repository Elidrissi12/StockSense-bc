using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagement.Application.Interfaces;
using StockManagement.Infrastructure.Communication;
using StockManagement.Infrastructure.DbContext;
using StockManagement.Infrastructure.Repositories;
using StockManagement.Infrastructure.Reports;
using StockManagement.Infrastructure.Security;

namespace StockManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:Default manquant dans appsettings.");

        services.AddDbContext<StockDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IStoredFileRepository, StoredFileRepository>();
        services.AddScoped<IPdfReportGenerator, PdfReportGenerator>();
        services.AddScoped<IExcelReportGenerator, ExcelReportGenerator>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}

