using Microsoft.Extensions.Logging;
using Products.Domain.Entities;

namespace Products.Infrastructure;

public static class DbInitializer
{
    public static void Seed(DatabaseContext _context, ILogger logger)
    {
        _context.Database.EnsureCreated();

        // Categories
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Kitchenware", Description = "Utensils, cookware, and tools used for cooking and food preparation." },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel and garments for men, women, and children." },
            new Category { Id = 3, Name = "Books", Description = "Printed and digital books across genres such as fiction, non-fiction, and academic." },
            new Category { Id = 4, Name = "Home Appliances", Description = "Electrical devices and machines for household tasks and convenience." },
            new Category { Id = 5, Name = "Electronics", Description = "Consumer electronic devices including phones, laptops, and accessories." }
        };

        foreach (var category in categories)
        {
            if (!_context.Categories.Any(c => c.Id == category.Id))
            {
                _context.Categories.Add(category);
                logger.LogInformation($"Seeded Category: {category.Name}");
            }
        }

        _context.SaveChanges();

        // Products
        var products = new List<Product>
        {
            new Product
            {
                Id = 100001,
                Name = "Non-stick Frying Pan",
                Description = "Durable non-stick pan suitable for everyday cooking.",
                CategoryId = 1,
                Stock = 50,
                Price = 1200.00m,
                CreatedBy = "Seeder",
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = 100002,
                Name = "Men’s Cotton T-Shirt",
                Description = "100% cotton round-neck T-shirt, breathable and comfortable.",
                CategoryId = 2,
                Stock = 200,
                Price = 499.00m,
                CreatedBy = "Seeder",
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = 100003,
                Name = "C# Programming Guide",
                Description = "Comprehensive guide to mastering C# and .NET development.",
                CategoryId = 3,
                Stock = 30,
                Price = 899.00m,
                CreatedBy = "Seeder",
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = 100004,
                Name = "Microwave Oven",
                Description = "800W compact microwave oven with multiple cooking modes.",
                CategoryId = 4,
                Stock = 15,
                Price = 7500.00m,
                CreatedBy = "Seeder",
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = 100005,
                Name = "Smartphone",
                Description = "Latest Android smartphone with 128GB storage and 5G support.",
                CategoryId = 5,
                Stock = 25,
                Price = 29999.00m,
                CreatedBy = "Seeder",
                CreatedDate = DateTime.UtcNow
            }
        };

        foreach (var product in products)
        {
            if (!_context.Products.Any(p => p.Id == product.Id))
            {
                _context.Products.Add(product);
                logger.LogInformation($"Seeded Product: {product.Name}");
            }
        }

        _context.SaveChanges();
        logger.LogInformation("Database seeding completed.");
    }
}
