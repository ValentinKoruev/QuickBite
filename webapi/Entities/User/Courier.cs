namespace QuickBite.Entities;
public class Courier : User
{
    public required string Name { get; set; }
    public string? Surname { get; set; } = null;
}
