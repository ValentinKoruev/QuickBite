namespace QuickBite.Entities;
public class Administrator : User
{
    public required string Name { get; set; }
    public string? Surname { get; set; } = null;
}
