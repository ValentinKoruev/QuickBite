using System.ComponentModel.DataAnnotations;

namespace QuickBite.Entities;
public class User
{
    public enum UserType 
    {
        Customer = 1,
        Courier = 2,
        Administrator = 4
    }

    [Key]
    public int Id { get; set; }
    public required UserType Type { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; } = null;
    public string? Address { get; set;} = null;
}