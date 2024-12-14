using System.ComponentModel.DataAnnotations;

namespace QuickBite.Models.Tokens;
public class CreateModel
{
    [Required(ErrorMessage = "This field is Required!")]
    public string Username { get; set; }

    [Required(ErrorMessage = "This field is Required!")]
    public string Password { get; set; }
}

