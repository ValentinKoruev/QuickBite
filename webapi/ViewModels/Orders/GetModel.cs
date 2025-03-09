using System.ComponentModel.DataAnnotations;
using QuickBite.Entities;
using static QuickBite.Entities.Order;

namespace QuickBite.Models.Orders;

public class GetModel
{
    public required Order Order { get; set; }
    public Product[] Products { get; set; } = [];
}