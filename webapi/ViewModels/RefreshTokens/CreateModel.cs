using System.ComponentModel.DataAnnotations;

namespace QuickBite.Models.RefreshTokens;

public class CreateModel
{
    [Required(ErrorMessage = "This field is Required!")]
    public string RefreshToken { get; set; }
}

