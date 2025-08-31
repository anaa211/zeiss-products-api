using AutoMapper;
using Moq;
using Products.Application.Models.ResponseModels;
using Products.Domain.Entities;

namespace Products.TestUtils;

public static class MapperMockFactory
{
    public static Mock<IMapper> Create()
    {
        var mapperMock = new Mock<IMapper>();

        // Product -> ProductResponseModel
        mapperMock
            .Setup(m => m.Map<ProductResponseModel>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductResponseModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category!.Name,
                Stock = p.Stock,
                Price = p.Price,
                CreatedBy = p.CreatedBy,
                CreatedDate = p.CreatedDate
            });

        // List<Product> -> List<ProductResponseModel>
        mapperMock
            .Setup(m => m.Map<List<ProductResponseModel>>(It.IsAny<List<Product>>()))
            .Returns((List<Product> list) => list.Select(p => new ProductResponseModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category!.Name,
                Stock = p.Stock,
                Price = p.Price,
                CreatedBy = p.CreatedBy,
                CreatedDate = p.CreatedDate
            }).ToList());

        return mapperMock;
    }
}

