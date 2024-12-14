namespace QuickBite.Models.Users;

public class CreateUserModel
{
    public enum UserType {
        BaseUser = 0,
        Customer = 1,
        Courier = 2,
        Restaurant = 3,
        Administrator = 4,
    }
    public required UserType Type { get; set;}
    public required string Username { get; set;} 
    public required string Password { get; set;}
    public required string Email { get; set;}
    public string? Address { get; set;}  
    public string? Name { get; set; }
    public string? Surname { get; set;}

}
