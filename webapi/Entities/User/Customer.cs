namespace QuickBite.Entities;
public class Customer : User
{
    public required string Name { get; set; }
    public string? Surname { get; set; } = null;
    public string? Address { get; set; } = null;
}
