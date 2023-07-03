using Microsoft.EntityFrameworkCore;
using ShopBackend.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
   options => options.UseSqlite($"Data Source={dbPath}"));


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
app.MapPut("/add_product", AddProduct);

async Task AddProduct(Product product, AppDbContext dbContext)
{
	await dbContext.Products.AddAsync(product);
	await dbContext.SaveChangesAsync();
}

Task<Product[]> GetProducts(AppDbContext dbContext)
{
	return dbContext.Products.ToArrayAsync();
}


app.Run();
