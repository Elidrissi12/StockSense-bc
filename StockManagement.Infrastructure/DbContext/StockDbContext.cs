using Microsoft.EntityFrameworkCore;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Enums;

namespace StockManagement.Infrastructure.DbContext;

public class StockDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nom).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(320).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.Role).IsRequired();
            e.Property(x => x.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nom).HasMaxLength(200).IsRequired();
            e.Property(x => x.Reference).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Reference).IsUnique();

            e.Property(x => x.Unite).HasMaxLength(30).IsRequired();
            e.Property(x => x.Localisation).HasMaxLength(200).IsRequired();

            e.Property(x => x.Quantite).HasPrecision(18, 4);
            e.Property(x => x.SeuilMin).HasPrecision(18, 4);

            e.HasOne(x => x.Categorie)
                .WithMany()
                .HasForeignKey(x => x.CategorieId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockMovement>(e =>
        {
            e.ToTable("StockMovements");
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).IsRequired();
            e.Property(x => x.Quantite).HasPrecision(18, 4);
            e.Property(x => x.DateUtc).IsRequired();

            e.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Alert>(e =>
        {
            e.ToTable("Alerts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).IsRequired();
            e.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            e.Property(x => x.DateUtc).IsRequired();

            e.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoredFile>(e =>
        {
            e.ToTable("StoredFiles");
            e.HasKey(x => x.Id);

            e.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
            e.Property(x => x.Tag).HasMaxLength(80).IsRequired();
            e.Property(x => x.SizeBytes).IsRequired();
            e.Property(x => x.Data).IsRequired();
            e.Property(x => x.CreatedAtUtc).IsRequired();

            e.HasIndex(x => x.Tag);
            e.HasIndex(x => x.CreatedAtUtc);

            e.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}

