using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Products.Application.Interfaces.Helpers;
using Products.Application.Interfaces.Managers;
using Products.Application.Models.RequestModels;
using Products.Application.Models.ResponseModels;
using Products.Domain.Entities;
using Products.Infrastructure;

namespace Products.Application.Managers;

public class ProductManager : IProductManager
{
    private readonly DatabaseContext _databaseContext;
    private readonly IMapper _mapper;
    private readonly IRandomIdGenerator _randomIdGenerator;
    private readonly ILogger<ProductManager> _logger;

    public ProductManager(
        DatabaseContext databaseContext,
        IMapper mapper,
        IRandomIdGenerator randomIdGenerator,
        ILogger<ProductManager> logger
    )
    {
        _databaseContext = databaseContext;
        _mapper = mapper;
        _randomIdGenerator = randomIdGenerator;
        _logger = logger;
    }

    public async Task<List<ProductResponseModel>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _databaseContext.Products
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<ProductResponseModel>>(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving products.");
            throw;
        }
    }

    public async Task<ProductResponseModel?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _databaseContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return _mapper.Map<ProductResponseModel>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving product with ID {id}.");
            throw;
        }
    }

    public async Task<List<ProductResponseModel>> AddProductsAsync(List<ProductRequestModel> products, CancellationToken cancellationToken = default)
    {
        try
        {
            if (products == null || !products.Any())
            {
                throw new ArgumentException("Product list cannot be empty.", nameof(products));
            }

            var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
            var existingCategories = await _databaseContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var invalidCategoryIds = categoryIds.Except(existingCategories).ToList();
            if (invalidCategoryIds.Any())
            {
                throw new KeyNotFoundException($"Invalid CategoryId(s): {string.Join(", ", invalidCategoryIds)}");
            }

            var productsList = new List<Product>();
            foreach (var item in products)
            {
                ValidateProductRequestData(item);

                var product = new Product
                {
                    Id = await _randomIdGenerator.GenerateUniqueIdAsync(),
                    Name = item.Name,
                    Description = item.Description,
                    CategoryId = item.CategoryId,
                    Stock = item.Stock,
                    Price = item.Price,
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow
                };
                productsList.Add(product);
            }
            await _databaseContext.Products.AddRangeAsync(productsList, cancellationToken);
            await _databaseContext.SaveChangesAsync(cancellationToken);

            var categoriesDict = await _databaseContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c, cancellationToken);

            foreach (var product in productsList)
            {
                product.Category = categoriesDict[product.CategoryId];
            }

            return _mapper.Map<List<ProductResponseModel>>(productsList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding products.");
            throw;
        }
    }

    public async Task<ProductResponseModel> UpdateProductAsync(int id, ProductRequestModel updatedProduct, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(id));

            if (updatedProduct == null)
                throw new ArgumentNullException(nameof(updatedProduct));

            var product = await _databaseContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            var categoryExists = await _databaseContext.Categories
                .AnyAsync(c => c.Id == updatedProduct.CategoryId, cancellationToken);
            if (!categoryExists)
                throw new KeyNotFoundException($"Category with ID {updatedProduct.CategoryId} not found.");

            ValidateProductRequestData(updatedProduct);

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.CategoryId = updatedProduct.CategoryId;
            product.Stock = updatedProduct.Stock;
            product.Price = updatedProduct.Price;
            product.ModifiedBy = "System";
            product.ModifiedDate = DateTime.UtcNow;

            await _databaseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ProductResponseModel>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating product with ID {id}.");
            throw;
        }
    }

    private static void ValidateProductRequestData(ProductRequestModel product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required.");

        if (product.Name.Length > 100)
            throw new ArgumentException("Product name cannot exceed 100 characters.");

        if (!string.IsNullOrEmpty(product.Description) && product.Description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters.");

        if (product.CategoryId <= 0)
            throw new ArgumentException("CategoryId must be valid.");

        if (product.Stock < 0)
            throw new ArgumentException("Stock cannot be negative.");

        if (product.Price <= 0)
            throw new ArgumentException("Price must be greater than 0.");

        return;
    }

    public async Task<ProductResponseModel> DecrementProductStock(int id, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(id));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var product = await _databaseContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            var stock = product.Stock;
            if (stock == 0)
                throw new InvalidOperationException($"No stock available for the product with ID {id}.");
            else if (stock - quantity < 0)
                throw new InvalidOperationException($"Stock available ({product.Stock}) is less than requested quantity ({quantity}) for product ID {id}.");
            else
            {
                product.Stock -= quantity;
                product.ModifiedBy = "System";
                product.ModifiedDate = DateTime.UtcNow;

                await _databaseContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Stock decremented by {quantity} for product with ID {id}. New stock: {product.Stock}");
                return _mapper.Map<ProductResponseModel>(product);
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while decrementing stock for product with ID {id}.");
            throw;
        }
    }

    public async Task<ProductResponseModel> IncrementProductStock(int id, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(id));

            if (quantity <= 0)
                throw new ArgumentException("Quantity to increment must be greater than zero.", nameof(quantity));

            var product = await _databaseContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            
            product.Stock = product.Stock + quantity;
            product.ModifiedBy = "System";
            product.ModifiedDate = DateTime.UtcNow;

            await _databaseContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Stock incremented by {quantity} for product with ID {id}. New stock: {product.Stock}");
            return _mapper.Map<ProductResponseModel>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while incrementing stock for product with ID {id}.");
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(id));

            var product = await _databaseContext.Products.FindAsync([id], cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            _databaseContext.Products.Remove(product);
            await _databaseContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Product with ID {id} deleted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting product with ID {id}.");
            throw;
        }
    }
}
