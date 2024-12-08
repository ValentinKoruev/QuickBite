using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickBite.Entities;
public class Order
{
    public enum OrderStatus {
        Pending = 0,
        Confirmed = 1,
        Preparing = 2,
        ReadyForPickup = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Canceled = 6,
        Failed = 7,
        Refunded = 8
    }

    [Key]
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required int RestaurantId { get; set; }
    public required string Address { get; set; }
    public required OrderStatus Status { get; set; } = OrderStatus.Pending;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    [ForeignKey(nameof(RestaurantId))]
    public Restaurant Restaurant { get; set; }
}