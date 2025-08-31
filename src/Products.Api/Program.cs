using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Products.Api.Middleware;
using Products.Application.Helpers;
using Products.Application.Interfaces.Helpers;
using Products.Application.Interfaces.Managers;
using Products.Application.Managers;
using Products.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services to the container
builder.Services.AddAutoMapper(
    cfg => { },  // optional config lambda
    AppDomain.CurrentDomain.GetAssemblies()
);
builder.Services.AddScoped<IRandomIdGenerator, RandomIdGenerator>();
builder.Services.AddScoped<IProductManager, ProductManager>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Products API",
        Version = "v1"
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API v1");
        c.RoutePrefix = string.Empty;
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DatabaseContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    DbInitializer.Seed(dbContext, logger);
}

app.Run();
