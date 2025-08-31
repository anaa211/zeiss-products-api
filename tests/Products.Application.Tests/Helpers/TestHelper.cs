using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Products.Application.Models.RequestModels;
using Products.Domain.Entities;
using Products.Infrastructure;
using Products.Application.Models.ResponseModels;


namespace Products.Application.Tests;

public static class TestHelper
{
    public static DatabaseContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DatabaseContext(options);
    }

    //public static IMapper GetMapper()
    //{
    //    var config = new AutoMapper.MapperConfiguration(cfg =>
    //    {
    //        cfg.CreateMap<Product, ProductResponseModel>()
    //            .ForMember(dest => dest.Category,
    //                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

    //        cfg.CreateMap<ProductRequestModel, Product>();
    //    });

    //    // Validate mapping
    //    config.AssertConfigurationIsValid();

    //    return config.CreateMapper();
    //}
}
