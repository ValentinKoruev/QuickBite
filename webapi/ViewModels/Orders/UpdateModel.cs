using System.ComponentModel.DataAnnotations;
using static QuickBite.Entities.Order;

namespace QuickBite.Models.Orders
{
  public class UpdateModel
  {
    [Required(ErrorMessage = "This field is Required!")]
    public int Id { get; set; }
    [Required(ErrorMessage = "This field is Required!")]
    public OrderStatus Status { get; set; }
  }
}