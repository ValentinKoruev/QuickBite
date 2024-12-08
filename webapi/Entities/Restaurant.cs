using System.ComponentModel.DataAnnotations;

namespace QuickBite.Entities;
public class Restaurant
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
}