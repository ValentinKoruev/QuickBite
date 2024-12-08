using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickBite.Entities;
public class Product
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Image { get; set; } = null;
    public required int RestaurantId { get; set; }

    [ForeignKey(nameof(RestaurantId))]
    public Restaurant Restaurant { get; set; }
}