using Products.Application.Models.RequestModels;
using Products.Application.Models.ResponseModels;

namespace Products.Application.Interfaces.Managers;

public interface IProductManager
{
    Task<List<ProductResponseModel>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductResponseModel?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ProductResponseModel>> AddProductsAsync(List<ProductRequestModel> products, CancellationToken cancellationToken = default);
    Task<ProductResponseModel> UpdateProductAsync(int id, ProductRequestModel updatedProduct, CancellationToken cancellationToken = default);
    Task<ProductResponseModel> DecrementProductStock(int id, int quantity, CancellationToken cancellationToken = default);
    Task<ProductResponseModel> IncrementProductStock(int id, int quantity, CancellationToken cancellationToken = default);
    Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
}
