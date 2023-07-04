using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
   options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/get_products", GetProducts);
app.MapGet("/get_product", GetProduct);
app.MapPost("/add_product", AddProduct);
app.MapPost("/update_product", UpdateProduct);
app.MapPost("/delete_product", DeleteProduct);

Task<Product[]> GetProducts(AppDbContext dbContext)
{
    return dbContext.Products.ToArrayAsync();
}

async Task<Product> GetProduct([FromQuery] Guid productId, AppDbContext dbContext)
{
    return await dbContext.Products.FindAsync(productId);
 
}

async Task AddProduct(Product product, AppDbContext dbContext, HttpContext context)
{
	await dbContext.Products.AddAsync(product);
	await dbContext.SaveChangesAsync();
    context.Response.StatusCode = StatusCodes.Status201Created;
}

async Task UpdateProduct([FromQuery] Guid productId, [FromBody] Product updatedProduct, AppDbContext dbContext, HttpContext context)
{
    var product = await dbContext.Products.FindAsync(productId);
    if (product != null)
    {
        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        await dbContext.SaveChangesAsync();
        context.Response.StatusCode = StatusCodes.Status200OK;
    }
}

async Task DeleteProduct([FromQuery] Guid productId, AppDbContext dbContext, HttpContext context)
{
    var product = await dbContext.Products.FindAsync(productId);
    if (product != null)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        context.Response.StatusCode = StatusCodes.Status204NoContent;
    }
}


app.Run();
