using System.ComponentModel.DataAnnotations;

namespace Products.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    //Navigation properties
    public List<Product> Products { get; set; } = new List<Product>();
}