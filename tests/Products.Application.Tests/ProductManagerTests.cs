using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Products.Application.Interfaces.Helpers;
using Products.Application.Managers;
using Products.Application.Models.RequestModels;
using Products.Domain.Entities;
using Products.Infrastructure;
using Products.TestUtils;

namespace Products.Tests.Managers
{
    public class ProductManagerTests : IDisposable
    {
        private readonly DatabaseContext _dbContext;
        private readonly Mock<IRandomIdGenerator> _randomIdGeneratorMock;
        private readonly Mock<ILogger<ProductManager>> _loggerMock;

        public ProductManagerTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _dbContext = new DatabaseContext(options);

            // seed a category
            _dbContext.Categories.Add(new Category { Id = 1, Name = "Test Category" });
            _dbContext.SaveChanges();

            _randomIdGeneratorMock = new Mock<IRandomIdGenerator>();
            _randomIdGeneratorMock.Setup(x => x.GenerateUniqueIdAsync()).ReturnsAsync(123);

            _loggerMock = new Mock<ILogger<ProductManager>>();
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProducts()
        {
            // Arrange
            _dbContext.Products.Add(new Product { Id = 123, Name = "P1", CategoryId = 1, Price = 100, Stock = 10 });
            _dbContext.SaveChanges();

            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            // Act
            var result = await manager.GetAllProductsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("P1", result[0].Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            _dbContext.Products.Add(new Product { Id = 456, Name = "P2", CategoryId = 1, Price = 200, Stock = 5 });
            _dbContext.SaveChanges();

            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            // Act
            var result = await manager.GetProductByIdAsync(456);

            // Assert
            Assert.Equal("P2", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldThrow_WhenNotFound()
        {
            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                manager.GetProductByIdAsync(999));
        }

        [Fact]
        public async Task AddProductsAsync_ShouldAddProducts()
        {
            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            var request = new List<ProductRequestModel>
            {
                new ProductRequestModel { Name = "NewProduct", CategoryId = 1, Stock = 10, Price = 100, Description = "desc" }
            };

            var result = await manager.AddProductsAsync(request);

            Assert.Single(result);
            Assert.Equal("NewProduct", result[0].Name);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdate_WhenValid()
        {
            _dbContext.Products.Add(new Product { Id = 789, Name = "Old", CategoryId = 1, Price = 50, Stock = 2 });
            _dbContext.SaveChanges();

            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            var update = new ProductRequestModel { Name = "Updated", CategoryId = 1, Stock = 20, Price = 150, Description = "new desc" };

            var result = await manager.UpdateProductAsync(789, update);

            Assert.Equal("Updated", result.Name);
            Assert.Equal(20, result.Stock);
        }

        [Fact]
        public async Task DecrementProductStock_ShouldReduceStock()
        {
            _dbContext.Products.Add(new Product { Id = 101, Name = "PStock", CategoryId = 1, Price = 100, Stock = 10 });
            _dbContext.SaveChanges();

            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            var result = await manager.DecrementProductStock(101, 3);

            Assert.Equal(7, result.Stock);
        }

        [Fact]
        public async Task DecrementProductStock_ShouldThrow_WhenStockTooLow()
        {
            _dbContext.Products.Add(new Product { Id = 202, Name = "LowStock", CategoryId = 1, Price = 100, Stock = 2 });
            _dbContext.SaveChanges();

            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                manager.DecrementProductStock(202, 5));
        }

        [Fact]
        public async Task IncrementProductStock_ShouldIncreaseStock()
        {
            _dbContext.Products.Add(new Product { Id = 303, Name = "IncStock", CategoryId = 1, Price = 100, Stock = 5 });
            _dbContext.SaveChanges();

            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            var result = await manager.IncrementProductStock(303, 10);

            Assert.Equal(15, result.Stock);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldDelete()
        {
            _dbContext.Products.Add(new Product { Id = 404, Name = "DeleteMe", CategoryId = 1, Price = 10, Stock = 1 });
            _dbContext.SaveChanges();

            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            var result = await manager.DeleteProductAsync(404);

            Assert.True(result);
            Assert.Empty(_dbContext.Products.ToList());
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldThrow_WhenNotFound()
        {
            var manager = new ProductManager(
                _dbContext,
                MapperMockFactory.Create().Object,
                _randomIdGeneratorMock.Object,
                _loggerMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                manager.DeleteProductAsync(999));
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}