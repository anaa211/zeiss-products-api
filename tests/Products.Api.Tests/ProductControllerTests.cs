using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Products.Api.Controllers;
using Products.Application.Interfaces.Managers;
using Products.Application.Models.RequestModels;
using Products.Application.Models.ResponseModels;
using Products.TestUtils;

namespace Products.Api.Tests;
public class ProductControllerTests
{
    private readonly Mock<IProductManager> _mockManager;
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockManager = new Mock<IProductManager>();
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockManager.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkAndLogs()
    {
        // Arrange
        _mockManager.Setup(m => m.GetAllProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponseModel>());

        // Act
        var result = await _controller.GetProducts(CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockLogger.VerifyLog(LogLevel.Debug, "GetProducts called in ProductController", Times.Once());
    }

    [Fact]
    public async Task GetProductById_ReturnsOkAndLogs()
    {
        // Arrange
        var product = new ProductResponseModel { Id = 1, Name = "Test" };
        _mockManager.Setup(m => m.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProductsById(1, CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockLogger.VerifyLog(LogLevel.Debug, "GetProductsById called in ProductController", Times.Once());
    }

    [Fact]
    public async Task CreateProducts_ReturnsOkAndLogs()
    {
        // Arrange
        var products = new List<ProductRequestModel>
        {
            new ProductRequestModel { Name = "Test", CategoryId = 1, Stock = 10, Price = 100 }
        };
        _mockManager.Setup(m => m.AddProductsAsync(products, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponseModel>());

        // Act
        var result = await _controller.CreateProducts(products, CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockLogger.VerifyLog(LogLevel.Debug, "CreateProducts called in ProductController", Times.Once());
    }

    [Fact]
    public async Task UpdateProduct_ReturnsOkAndLogs()
    {
        // Arrange
        var request = new ProductRequestModel { Name = "Updated", CategoryId = 1, Stock = 5, Price = 200 };
        var updated = new ProductResponseModel { Id = 1, Name = "Updated" };

        _mockManager.Setup(m => m.UpdateProductAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        // Act
        var result = await _controller.UpdateProduct(1, request, CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockLogger.VerifyLog(LogLevel.Debug, "UpdateProduct called in ProductController", Times.Once());
    }

    [Fact]
    public async Task DecrementProductStock_ReturnsOkAndLogs()
    {
        // Arrange
        var response = new ProductResponseModel { Id = 1, Stock = 5 };
        _mockManager.Setup(m => m.DecrementProductStock(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.DecrementProductStock(1, 2, CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockLogger.VerifyLog(LogLevel.Debug, "DecrementProductStock called in ProductController", Times.Once());
    }

    [Fact]
    public async Task IncrementProductStock_ReturnsOkAndLogs()
    {
        // Arrange
        var response = new ProductResponseModel { Id = 1, Stock = 15 };
        _mockManager.Setup(m => m.IncrementProductStock(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.IncrementProductStock(1, 10, CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockLogger.VerifyLog(LogLevel.Debug, "IncrementProductStock called in ProductController", Times.Once());
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContentAndLogs()
    {
        // Arrange
        _mockManager.Setup(m => m.DeleteProductAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(1, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockLogger.VerifyLog(LogLevel.Debug, "DeleteProduct called in ProductController", Times.Once());
    }

    [Fact]
    public async Task DeleteProduct_ThrowsException_LogsError()
    {
        // Arrange
        _mockManager.Setup(m => m.DeleteProductAsync(100, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.DeleteProduct(100, CancellationToken.None));

        _mockLogger.VerifyLog(LogLevel.Debug, "DeleteProduct called in ProductController", Times.Once());
    }
}

