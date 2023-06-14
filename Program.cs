using OnlineShopPoC;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

Catalog catalog = new Catalog();

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

async Task<IResult> AddProductAsync(Product product, HttpContext context)
{
    await catalog.AddProduct(product);
    return Results.Created($"/products/{product.Id}", product);
}

async Task<List<Product>> GetProductsAsync()
{
    var dictionary = await catalog.GetProductsAsync();
    return dictionary.Values.ToList();
}

async Task<Product> GetProductByIdAsync(string id)
{
    return await catalog.GetProductByIdAsync(Guid.Parse(id));
}

async Task<IResult> UpdateProductByIdAsync(string productId, Product newProduct)
{
    await catalog.UpdateProductById(Guid.Parse(productId), newProduct);
    return Results.Accepted($"/products/{newProduct.Id}");
}

async Task<IResult> DeleteProductByIdAsync(string productId)
{
    await catalog.DeleteProductById(Guid.Parse(productId));
    return Results.Accepted($"/products/all");
}

async Task<IResult> ClearProductsAsync()
{
    await catalog.ClearProducts();
    return Results.Accepted($"/products/all");
}


app.Run();
