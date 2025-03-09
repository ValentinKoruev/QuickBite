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

        var userCred = User.FindFirst("LoggedUserId");

        if(userCred is null) return Unauthorized();
        
        int loggedUserId = Convert.ToInt32(userCred.Value);

        User loggedUser = context.Users
        .Where(u => u.Id == loggedUserId)
        .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        List<Order> res = [];
        switch(loggedUser) {
            case Customer: {
                res = context.Orders.Where(o => o.UserId == loggedUserId).ToList();
                break;
            }
            case Courier: {
                res = context.Orders.Where(o => o.CourierId == loggedUserId).ToList();
                break;
            }
            case Restaurant: {
                res = context.Orders.Where(o => o.RestaurantId == loggedUserId).ToList();
                break;
            }
            case Administrator: {
                res = context.Orders.ToList();
                break;
            }
        }

        List<GetModel> getModels = new List<GetModel>();
        foreach(var order in res) {
            Product[] orderProducts = context.OrderProducts.Where(op => op.OrderId == order.Id).Select(op => op.Product).ToArray();
            getModels.Add(new GetModel() {
                Order = order,
                Products = orderProducts
            });
        }

        context.Dispose();

        return Ok(getModels);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        var userCred = User.FindFirst("LoggedUserId");

        if(userCred is null) return Unauthorized();
         
        int loggedUserId = Convert.ToInt32(userCred.Value);

        User loggedUser = context.Users
        .Where(u => u.Id == loggedUserId)
        .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        Order order = null;

        order = context.Orders.Where(o => o.Id == id).FirstOrDefault();

        if(order == null) return NotFound();

        switch(loggedUser) {
            case Customer: if(order.UserId != loggedUserId) return Forbid(); break;
            case Courier: if(order.CourierId != loggedUserId) return Forbid(); break;
            case Restaurant: if(order.RestaurantId != loggedUserId) return Forbid(); break;
        }

        Product[] orderProducts = context.OrderProducts.Where(op => op.OrderId == order.Id).Select(op => op.Product).ToArray();

        context.Dispose();
        
        return Ok(new GetModel {
            Order = order,
            Products = orderProducts
        });
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateModel model)
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        var userCred = User.FindFirst("LoggedUserId");

        if(userCred is null) return Unauthorized();
        
        int loggedUserId = Convert.ToInt32(userCred.Value);
        
        User loggedUser = context.Users
        .Where(u => u.Id == loggedUserId)
        .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        if (loggedUser is not Customer && loggedUser is not Administrator) return Forbid();

        Restaurant restaurant = context.Restaurants
        .Where(r => r.Id == model.RestaurantId)
        .FirstOrDefault();

        if (restaurant == null) return BadRequest();           

        Order order = new Order {
            UserId = loggedUserId,
            RestaurantId = model.RestaurantId,
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

        return Ok(order);
    }

    [HttpPut]
    public IActionResult Put([FromBody] UpdateModel model)
    {
        QuickBiteDbContext context = new QuickBiteDbContext();

        
        var userCred = User.FindFirst("LoggedUserId");

        if(userCred is null) return Unauthorized();
        
        int loggedUserId = Convert.ToInt32(userCred.Value);
        
        User loggedUser = context.Users
        .Where(u => u.Id == loggedUserId)
        .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        Order order = context.Orders
            .Where(i => i.Id == model.Id)
            .FirstOrDefault();

        if (order == null) return BadRequest();

        // administrator have full control over orders
        if(loggedUser is Administrator) {
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
                if(loggedUser is not Courier) 
                    return Forbid(); 

                order.CourierId = loggedUser.Id;
                
                break;
            }
            case OrderStatus.OutForDelivery:
            case OrderStatus.Delivered:
                if(loggedUser is not Courier || order.CourierId != loggedUserId) return Forbid(); break;
            
            // handled by restaurant
            case OrderStatus.Preparing:
            case OrderStatus.ReadyForPickup:
                if(loggedUser is not Restaurant || order.RestaurantId != loggedUserId) return Forbid(); break;

            // handled by customer
            case OrderStatus.Canceled:
                if(loggedUser is not Customer || order.UserId != loggedUserId) return Forbid(); break;
            
            // handled both by courier and restaurant
            case OrderStatus.Failed:
                if(loggedUser is not Courier || order.CourierId != loggedUserId)  
                    return Forbid();

                if(!(loggedUser is not Restaurant || order.RestaurantId != loggedUserId))  
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

        if (loggedUser is not Administrator) return Forbid();

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