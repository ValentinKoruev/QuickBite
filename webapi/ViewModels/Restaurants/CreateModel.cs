namespace QuickBite.Models.Restaurants;

public class CreateModel
{
    public int RestaurantId { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Image { get; set; }
}