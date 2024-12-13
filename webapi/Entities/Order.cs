using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickBite.Entities;
public class Order
{
    public enum OrderStatus {
        Pending = 0, // order sent by customer, awaiting courier confirmaion
        Confirmed = 1, // order confirmed by courier, awaiting restaurant preparation
        Preparing = 2, // product preparation
        ReadyForPickup = 3, // product ready for pickup by courier
        OutForDelivery = 4, // courier making his way to the address
        Delivered = 5, // delivered by courier
        Canceled = 6, // canceled by customer
        Failed = 7, // failed either by courier or restaurant, awaiting refund
        Refunded = 8 // refund, handled only by administrator level user or higher
    }

    [Key]
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required int RestaurantId { get; set; }
    public int? CourierId { get; set; } = null;
    public required string Address { get; set; }
    public required OrderStatus Status { get; set; } = OrderStatus.Pending;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    [ForeignKey(nameof(CourierId))]
    public User Courier { get; set; }
    [ForeignKey(nameof(RestaurantId))]
    public Restaurant Restaurant { get; set; }
}