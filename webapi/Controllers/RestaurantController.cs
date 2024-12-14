using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuickBite.Entities;
using QuickBite.Repositories;
using QuickBite.Models.Restaurants;

namespace QuickBite.Controllers;


[Route("api/[controller]")]
[ApiController]
public class RestaurantsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() {
        QuickBiteDbContext context = new QuickBiteDbContext();
        Restaurant[] restaurants = context.Restaurants.ToArray();

        List<GetModel> getModels = new List<GetModel>();
        foreach(Restaurant restaurant in restaurants) {
            Product[] products = context.Products.Where(p => p.RestaurantId == restaurant.Id).ToArray();
            getModels.Add(new GetModel {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                Products = products
            });
        }

        return Ok(getModels);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id) {
        QuickBiteDbContext context = new QuickBiteDbContext();

        Restaurant restaurant = context.Restaurants.Where(r => r.Id == id).FirstOrDefault();

        if(restaurant == null) return NotFound();

        Product[] products = context.Products.Where(p => p.RestaurantId == restaurant.Id).ToArray();

        return Ok(new GetModel {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Address = restaurant.Address,
            Products = products
        });
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateModel model) {
        QuickBiteDbContext context = new QuickBiteDbContext();

        int loggedUserId = -6;

        User loggedUser = context.Users
                .Where(u => u.Id == loggedUserId)
                .FirstOrDefault();

        if (loggedUser == null) return Unauthorized();

        if(loggedUser is not Administrator) 
            if(loggedUser is not Restaurant || loggedUserId != model.RestaurantId) return Forbid();

        var restaurant = context.Restaurants.Where(r => r.Id == model.RestaurantId).FirstOrDefault();

        if(restaurant is null) return BadRequest();

        context.Products.Add(new Product {
            RestaurantId = model.RestaurantId,
            Name = model.Name,
            Price = model.Price,
            Image = model.Image,
        });

        context.SaveChanges();

        context.Dispose();

        return Ok();
    }
}