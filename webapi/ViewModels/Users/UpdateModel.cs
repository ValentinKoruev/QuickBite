namespace QuickBite.Models.Users;

public class UpdateUserModel
{
    public enum UserType {
        BaseUser = 0,
        Customer = 1,
        Courier = 2,
        Restaurant = 3,
        Administrator = 4,
    }
    public required int Id { get; set; } 
    public UserType? Type { get; set;}
    public string? Username { get; set;} 
    public string? Password { get; set;}
    public string? Email { get; set;}
    public string? Address { get; set;}  
    public string? Name { get; set; }
    public string? Surname { get; set;}

}
