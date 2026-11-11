using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_ivanov.Data;
using wt_lab7_7postgres_efcore_ivanov.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Получаем строку подключения из appsettings.Development.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Если мы на Render, переопределяем строку через DATABASE_URL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    
    // Проверка порта (если Render не передал, ставим 5432)
    var dbPort = uri.Port == -1 ? 5432 : uri.Port; 
    
    connectionString = $"Host={uri.Host};Port={dbPort};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true;";
}

// 3. Подключаем базу данных и OpenAPI
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddOpenApi();

var app = builder.Build();

// 4. Автоматически применяем миграции при запуске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 5. Включаем OpenAPI в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ==================== ЭНДПОИНТЫ ====================

app.MapGet("/", () => "Minimal API + EF Core работает!");
app.MapGet("/api/info", () => new { Application = "Web Technologies", Technology = "ASP.NET Core + EF Core + PostgreSQL" });

// GET: Все товары
app.MapGet("/api/products", async (AppDbContext db) => 
    Results.Ok(await db.Products.ToListAsync()));

// GET: Поиск
app.MapGet("/api/products/search", async (string? search, AppDbContext db) =>
{
    var query = db.Products.AsQueryable();
    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
    return Results.Ok(await query.ToListAsync());
});

// GET: По ID
app.MapGet("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    return product == null ? Results.NotFound() : Results.Ok(product);
});

// POST: Создать
app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
});

// PUT: Изменить
app.MapPut("/api/products/{id}", async (int id, Product updated, AppDbContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    if (product == null) return Results.NotFound();

    product.Name = updated.Name;
    product.Category = updated.Category;
    product.Price = updated.Price;
    product.IsAvailable = updated.IsAvailable;
    
    await db.SaveChangesAsync();
    return Results.Ok(product);
});

// DELETE: Удалить
app.MapDelete("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    if (product == null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ==================== ЗАПУСК ====================
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");