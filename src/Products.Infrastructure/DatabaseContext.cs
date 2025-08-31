using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;

namespace Products.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .Property(c => c.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Category>()
           .Property(p => p.Name)
           .HasMaxLength(100);

        modelBuilder.Entity<Category>()
            .Property(p => p.Description)
            .HasMaxLength(500);

        modelBuilder.Entity<Product>()
            .Property(p => p.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .HasMaxLength(100);

        modelBuilder.Entity<Product>()
            .Property(p => p.Description)
            .HasMaxLength(500);
    }
}