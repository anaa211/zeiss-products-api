using AutoMapper;
using Products.Application.Models.RequestModels;
using Products.Application.Models.ResponseModels;
using Products.Domain.Entities;

namespace Products.Application.AutoMappers;

public class ProductProfile: Profile
{
    public ProductProfile()
    {
        CreateMap<ProductRequestModel, Product>();

        CreateMap<Product, ProductResponseModel>()
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
    }
}
