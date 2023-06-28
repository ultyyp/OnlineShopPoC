using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using OnlineShopPoC;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using System.Reflection;
using Serilog;
using OnlineShopPoC.Interfaces;
using OnlineShopPoC.Decorators;
using OnlineShopPoC.Services;
using OnlineShopPoC.Objects;
using Serilog.Events;
using Sentry;

Log.Logger = new LoggerConfiguration()
   .WriteTo.Sentry(o =>
   {
       // Debug and higher are stored as breadcrumbs (default is Information)
       o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
       // Warning and higher is sent as event (default is Error)
       o.MinimumEventLevel = LogEventLevel.Warning;
   })
   .CreateBootstrapLogger();
   Log.Information("Starting up");


try
{


    //Builder
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.WebHost.UseSentry(Environment.GetEnvironmentVariable("sentry_dsn")!);


    //Logger
    builder.Host.UseSerilog((_, c) =>
    {
        c.Enrich.FromLogContext()
            .MinimumLevel.Information() //<- Минимальный уровень для всех приемников
            .WriteTo.File(@"G:\ITSTEP\SP Projects\ASP.NET\Lessons\Lesson 2 - OnlineShopPoC\OnlineShopPoC\Logs\log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.Seq("http://localhost:5341")
            .WriteTo.Sentry(s =>
            {
                s.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                s.MinimumEventLevel = LogEventLevel.Warning;
            });
            

    });


    //Options
    builder.Configuration.AddUserSecrets<SendGridConfig>();


    //Singletons
    builder.Services.AddSingleton<ICatalog, InMemoryCatalog>();
    builder.Services.AddSingleton<IClock, CurrentClock>();

    //Scoped
    builder.Services.AddScoped<IEmailSender, SendGridEmailSender>();
    builder.Services.Decorate<IEmailSender, EmailSenderLoggingDecorator>();
    builder.Services.Decorate<IEmailSender, EmailSenderRetryDecorator>();

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

    //TESTING
    app.MapGet("/send_email", SendEmail);

    async Task SendEmail(IEmailSender emailSender)
    {
        await emailSender.SendEmailAsync(Environment.GetEnvironmentVariable("myemail2"), "Testing", "Test test test" + DateTime.Now.ToString());
    }


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

    app.UseSentryTracing();
    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Server Crashed!");
}
finally
{
    Log.Information("Shut down complete");
    await Log.CloseAndFlushAsync();
}

