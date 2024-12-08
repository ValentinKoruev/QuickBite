using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickBite.Entities;
public class OrderProduct
{
    [Key]
    public int Id { get; set; }
    public required int OrderId { get; set;}
    public required int ProductId { get; set; }
    public required string Name { get; set; }
    public required uint Quantity { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; }
}