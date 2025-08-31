
namespace Products.Application.Models.RequestModels;

public class ProductRequestModel
{
    public string Name { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int Stock { get; set; }

    public decimal Price { get; set; }
}
