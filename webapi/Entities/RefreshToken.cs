
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickBite.Entities;
public class RefreshToken
{
    public enum TokenStatus
    {
        Pending = 0,
        Used = 1
    }

    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public TokenStatus Status { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

}

