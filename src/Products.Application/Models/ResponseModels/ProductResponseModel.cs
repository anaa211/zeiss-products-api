
namespace Products.Application.Models.ResponseModels;

public class ProductResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string Category { get; set; }

    public int Stock { get; set; }

    public decimal Price { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }
}
