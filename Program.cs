using wt_lab6_6minimalapi_ivanov.Models;
using wt_lab6_6minimalapi_ivanov.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключаем OpenAPI (Swagger)
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 2. Базовые маршруты
app.MapGet("/", () => "Minimal API работает!");

app.MapGet("/api/info", () => new
{
    Application = "Web Technologies",
    Technology = "ASP.NET Core Minimal API",
    Version = "1.0"
});

// 3. GET: Получить все (с фильтрацией)
app.MapGet("/api/products", (string? search) =>
{
    var products = ProductStore.Products.AsEnumerable();
    if (!string.IsNullOrWhiteSpace(search))
    {
        products = products.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
    }
    return Results.Ok(products);
});

// 4. GET: Получить по ID
app.MapGet("/api/products/{id}", (int id) =>
{
    var product = ProductStore.Products.FirstOrDefault(x => x.Id == id);
    if (product == null) return Results.NotFound(new { error = "Товар не найден" });
    return Results.Ok(product);
});

// 5. POST: Создать товар
app.MapPost("/api/products", (Product product) =>
{
    if (string.IsNullOrWhiteSpace(product.Name) || product.Price <= 0)
        return Results.BadRequest(new { error = "Некорректные данные" });

    product.Id = ProductStore.Products.Count == 0 ? 1 : ProductStore.Products.Max(x => x.Id) + 1;
    ProductStore.Products.Add(product);
    
    return Results.Created($"/api/products/{product.Id}", product);
});

// 6. PUT: Изменить товар
app.MapPut("/api/products/{id}", (int id, Product updatedProduct) =>
{
    var product = ProductStore.Products.FirstOrDefault(x => x.Id == id);
    if (product == null) return Results.NotFound();

    product.Name = updatedProduct.Name;
    product.Category = updatedProduct.Category;
    product.Price = updatedProduct.Price;
    product.IsAvailable = updatedProduct.IsAvailable;

    return Results.Ok(product);
});

// 7. DELETE: Удалить товар
app.MapDelete("/api/products/{id}", (int id) =>
{
    var product = ProductStore.Products.FirstOrDefault(x => x.Id == id);
    if (product == null) return Results.NotFound();

    ProductStore.Products.Remove(product);
    return Results.NoContent(); // 204
});

// Настройка порта для Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");