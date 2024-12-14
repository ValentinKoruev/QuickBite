using QuickBite.Entities;

namespace QuickBite.Models.Restaurants;

public class GetModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public Product[] Products { get; set; } = [];
}