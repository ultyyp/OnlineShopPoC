using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using OnlineShopPoC;
using System.Collections.Generic;
using MailKit.Net.Smtp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Singletons
builder.Services.AddSingleton<ICatalog, InMemoryCatalog>();
builder.Services.AddSingleton<IClock, CurrentClock>();
builder.Services.AddSingleton<IEmailSender, SendGridEmailSender>();

//Hosted Services
builder.Services.AddHostedService<AppStartedNotificatorBackgroundService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

//RPC
app.MapPost("/add_product", AddProductAsync); //C
app.MapGet("/get_products", GetProductsAsync); //R1
app.MapGet("/get_product", GetProductByIdAsync); //R2
app.MapPost("/update_product", UpdateProductByIdAsync); //U
app.MapPost("/delete_product", DeleteProductByIdAsync); //D
app.MapPost("/clear_products", ClearProductsAsync); //*

//REST
app.MapPost("/products", AddProductAsync); //C
app.MapGet("/products/all", GetProductsAsync); //R
app.MapPut("/products/{productId}", UpdateProductByIdAsync); //U
app.MapDelete("/products/{productId}", DeleteProductByIdAsync); //D

async Task<IResult> AddProductAsync(Product product, HttpContext context, ICatalog catalog)
{
    await catalog.AddProduct(product);
    return Results.Created($"/products/{product.Id}", product);
}

async Task<List<Product>> GetProductsAsync(ICatalog catalog, IClock clock)
{
    return await catalog.GetProductsAsync(clock); ;
}

async Task<Product> GetProductByIdAsync(string id, ICatalog catalog, IClock clock)
{
    return await catalog.GetProductByIdAsync(Guid.Parse(id), clock); ;
}

async Task<IResult> UpdateProductByIdAsync(string productId, Product newProduct, ICatalog catalog)
{
    await catalog.UpdateProductById(Guid.Parse(productId), newProduct);
    return Results.Accepted($"/products/{newProduct.Id}");
}

async Task<IResult> DeleteProductByIdAsync(string productId, ICatalog catalog)
{
    await catalog.DeleteProductById(Guid.Parse(productId));
    return Results.Accepted($"/products/all");
}

async Task<IResult> ClearProductsAsync(ICatalog catalog)
{
    await catalog.ClearProducts();
    return Results.Accepted($"/products/all");
}


app.Run();
