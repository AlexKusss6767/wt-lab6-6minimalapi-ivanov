using wt_lab6_6minimalapi_ivanov.Models;

namespace wt_lab6_6minimalapi_ivanov.Data;

public static class ProductStore
{
    public static List<Product> Products { get; } = new()
    {
        new Product { Id = 1, Name = "Ноутбук", Category = "Компьютеры", Price = 2500, IsAvailable = true },
        new Product { Id = 2, Name = "Мышь", Category = "Аксессуары", Price = 80, IsAvailable = true },
        new Product { Id = 3, Name = "Клавиатура", Category = "Аксессуары", Price = 150, IsAvailable = false }
    };
}