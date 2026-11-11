using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_ivanov.Models;

namespace wt_lab7_7postgres_efcore_ivanov.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    // Добавляем начальные данные (Seeding) - пункт 23 в методичке
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Ноутбук", Category = "Компьютеры", Price = 2500, IsAvailable = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 2, Name = "Мышь", Category = "Аксессуары", Price = 80, IsAvailable = true, CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}