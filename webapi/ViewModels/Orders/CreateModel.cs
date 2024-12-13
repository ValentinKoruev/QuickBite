using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration.UserSecrets;
using QuickBite.Entities;
using static QuickBite.Entities.Order;

namespace QuickBite.Models.Orders
{  
    public class CreateModel
    {
        public class ProductModel {
            public int Id { get; set; }
            public int Quantity { get; set; }
        } 
        // public int UserId { get; set; }
        public required int RestaurantId {get; set; }
        public required string Address { get; set;}
        public required ProductModel[] Products { get; set; }
    }
}