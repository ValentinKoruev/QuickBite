using Microsoft.AspNetCore.Mvc;
using QuickBite.Repositories;
using QuickBite.Entities;
using Microsoft.AspNetCore.Authorization;
using QuickBite.Models.Orders;
using static QuickBite.Entities.User;
using static QuickBite.Entities.Order;

namespace QuickBite.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{

    [HttpGet]
    public IActionResult Get()
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        int loggedUserId = -1;
        
        User loggedUser = context.Users
                                  .Where(u => u.Id == loggedUserId)
                                  .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        List<Order> res = [];
        switch(loggedUser.Type) {
            case UserType.Customer: {
                res = context.Orders.Where(o => o.UserId == loggedUserId).ToList();
                break;
            }
            case UserType.Courier: {
                res = context.Orders.Where(o => o.CourierId == loggedUserId).ToList();
                break;
            }
            case UserType.Restaurant: {
                res = context.Orders.Where(o => o.RestaurantId == loggedUserId).ToList();
                break;
            }
            case UserType.Administrator: {
                res = context.Orders.ToList();
                break;
            }
        }
        

        context.Dispose();

        return Ok(res);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        int loggedUserId = -1;

        User loggedUser = context.Users
                                  .Where(u => u.Id == loggedUserId)
                                  .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        Order res = null;

        res = context.Orders.Where(o => o.Id == id).FirstOrDefault();

        if(res == null) return NotFound();

        switch(loggedUser.Type) {
            case UserType.Customer: if(res.UserId != loggedUserId) return Forbid(); break;
            case UserType.Courier: if(res.CourierId != loggedUserId) return Forbid(); break;
            case UserType.Restaurant: if(res.RestaurantId != loggedUserId) return Forbid(); break;
        }
        context.Dispose();

        return Ok(res);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateModel model)
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        // int loggedUserId = Convert.ToInt32(this.User.FindFirst("loggedUserId").Value);
        int loggedUserId = -1;
        
        User loggedUser = context.Users
                                  .Where(u => u.Id == loggedUserId)
                                  .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        if (loggedUser.Type == UserType.Customer || loggedUser.Type == UserType.Administrator) return Forbid();

        Restaurant restaurant = context.Restaurants
                                  .Where(r => r.Id == model.RestaurantId)
                                  .FirstOrDefault();

        if (restaurant == null) return BadRequest();           

        Order order = new Order {
            UserId = loggedUserId,
            RestaurantId = context.Restaurants.First().Id,
            Address = model.Address,
            Status = OrderStatus.Pending
        };

        context.Orders.Add(order);

        context.SaveChanges();

        foreach(CreateModel.ProductModel productModel in model.Products) {

            Product product = context.Products
                                  .Where(p => p.Id == productModel.Id)
                                  .FirstOrDefault();

            if(product == null) continue; // skip not found items

            context.OrderProducts.Add(new OrderProduct {
                ProductId = product.Id,
                OrderId = order.Id,
                Quantity = (uint)productModel.Quantity
            });
        }
        context.SaveChanges();


        context.Dispose();

        return Ok();
    }

    [HttpPut]
    public IActionResult Put([FromBody] UpdateModel model)
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        int loggedUserId = -2;
        
        User loggedUser = context.Users
                                  .Where(u => u.Id == loggedUserId)
                                  .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        Order order = context.Orders
            .Where(i => i.Id == model.Id)
            .FirstOrDefault();

        if (order == null) return BadRequest();

        // administrator have full control over orders
        if(loggedUser.Type == UserType.Administrator) {
            order.Status = model.Status;
            context.Orders.Update(order);
            context.SaveChanges();

            context.Dispose();

            return Ok(order);
        }
         
        // if not administrator or higher, 
        // order status may only be modified one step at a time, 
        // unless it's Canceled or Failed
        if(
            (int)order.Status+1 != (int)model.Status
        &&  (int)model.Status+1 < (int)OrderStatus.Canceled
        ) return Forbid();

        switch(model.Status) {
            // handled by courier
            case OrderStatus.Confirmed: {
                if(loggedUser.Type != UserType.Courier) 
                    return Forbid(); 

                order.CourierId = loggedUser.Id;
                
                break;
            }
            case OrderStatus.OutForDelivery:
            case OrderStatus.Delivered:
                if(loggedUser.Type != UserType.Courier || order.CourierId != loggedUserId) return Forbid(); break;
            
            // handled by restaurant
            case OrderStatus.Preparing:
            case OrderStatus.ReadyForPickup:
                if(loggedUser.Type != UserType.Restaurant || order.RestaurantId != loggedUserId) return Forbid(); break;

            // handled by customer
            case OrderStatus.Canceled:
                if(loggedUser.Type != UserType.Customer || order.UserId != loggedUserId) return Forbid(); break;
            
            // handled both by courier and restaurant
            case OrderStatus.Failed:
                if(!(loggedUser.Type == UserType.Courier && order.CourierId == loggedUserId))  
                    return Forbid();

                if(!(loggedUser.Type == UserType.Restaurant && order.RestaurantId == loggedUserId))  
                    return Forbid();
                
                break;

            default: 
                return BadRequest();
        }

        order.Status = model.Status;
        context.Orders.Update(order);
        context.SaveChanges();

        context.Dispose();

        return Ok(order);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        int loggedUserId = -3;
        
        User loggedUser = context.Users
                                  .Where(u => u.Id == loggedUserId)
                                  .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        if (loggedUser.Type != UserType.Administrator) return Forbid();

        Order order = context.Orders
                            .Where(i => i.Id == id)
                            .FirstOrDefault();

        if (order != null)
        {
            context.Orders.Remove(order);
            context.SaveChanges();

            context.Dispose();

            return Ok(order);
        }

        context.Dispose();

        return NotFound();
    }
}