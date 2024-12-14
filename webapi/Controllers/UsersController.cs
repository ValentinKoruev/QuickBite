using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuickBite.Entities;
using QuickBite.Repositories;
using static QuickBite.Entities.User;
using QuickBite.Models.Users;
using static QuickBite.Models.Users.CreateUserModel;

namespace QuickBite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
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

            if(loggedUser is not Administrator) return Forbid();

            List<User> res = context.Users.ToList();
            
            context.Dispose();

            return Ok(res);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            QuickBiteDbContext context = new QuickBiteDbContext();

            User user = context.Users
            .Where(i => i.Id == id)
            .FirstOrDefault();

            context.Dispose();

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateUserModel user)
        {
            QuickBiteDbContext context = new QuickBiteDbContext();

            
            switch(user.Type) {
                case UserType.BaseUser: 
                {
                    context.Users.Add(new User() {
                        Username = user.Username,
                        Password = user.Password,
                        Email = user.Email
                    });
                    break;
                }
                case UserType.Customer: 
                {
                    if(user.Name is null) return BadRequest("Name is required.");

                    context.Customers.Add(new Customer() {
                        Username = user.Username,
                        Password = user.Password,
                        Email = user.Email,
                        Name = user.Name,
                        Surname = user.Surname,
                        Address = user.Address,
                    });
                    break;
                }
                case UserType.Courier: 
                {
                    if(user.Name is null) return BadRequest("Name is required.");

                    context.Couriers.Add(new Courier() {
                        Username = user.Username,
                        Password = user.Password,
                        Email = user.Email,
                        Name = user.Name,
                        Surname = user.Surname
                    });
                    break;
                }
                case UserType.Restaurant: 
                {
                    if(user.Name is null) return BadRequest("Name is required.");
                    if(user.Address is null) return BadRequest("Address is required.");

                    context.Restaurants.Add(new Restaurant() {
                        Username = user.Username,
                        Password = user.Password,
                        Email = user.Email,
                        Name = user.Name,
                        Address = user.Address,
                    });
                    break;
                }
                case UserType.Administrator: 
                {
                    if(user.Name is null) return BadRequest("Name is required.");

                    context.Administrators.Add(new Administrator() {
                        Username = user.Username,
                        Password = user.Password,
                        Email = user.Email,
                        Name = user.Name,
                        Surname = user.Surname,
                    });
                    break;
                }
            }

            
            context.SaveChanges();

            context.Dispose();

            return Ok(user);
        }

        [HttpPut]
        public IActionResult Put([FromBody] UpdateUserModel item)
        {
            QuickBiteDbContext context = new QuickBiteDbContext();

            int loggedUserId = -6;
        
            var loggedUser = context.Set<User>().Find(loggedUserId);

            if (loggedUser == null) return Unauthorized();

            if(loggedUser.Id != item.Id && loggedUser is not Administrator) return Forbid();

            // if we are logged as an administrator we need to find the user we want to update
            var user = loggedUser is Administrator 
            ? context.Users.Find(item.Id)
            : loggedUser; 

            switch(user) {
                case Customer: {
                    var cast = user as Customer;
                    if(item.Username is not null) cast.Username = item.Username;
                    if(item.Password is not null) cast.Password = item.Password;
                    if(item.Email is not null) cast.Email = item.Email;
                    if(item.Name is not null) cast.Name = item.Name;
                    if(item.Surname is not null) cast.Surname = item.Surname;
                    if(item.Address is not null) cast.Address = item.Address;
                    break;
                }
                case Courier: {
                    var cast = user as Courier;
                    if(item.Username is not null) cast.Username = item.Username;
                    if(item.Password is not null) cast.Password = item.Password;
                    if(item.Email is not null) cast.Email = item.Email;
                    if(item.Name is not null) cast.Name = item.Name;
                    if(item.Surname is not null) cast.Surname = item.Surname;
                    break;
                }
                case Restaurant: {
                    var cast = user as Restaurant;
                    if(item.Username is not null) cast.Username = item.Username;
                    if(item.Password is not null) cast.Password = item.Password;
                    if(item.Email is not null) cast.Email = item.Email;
                    if(item.Name is not null) cast.Name = item.Name;
                    if(item.Address is not null) cast.Address = item.Address;
                    break;
                }
                case Administrator: {
                    var cast = user as Administrator;
                    if(item.Username is not null) cast.Username = item.Username;
                    if(item.Password is not null) cast.Password = item.Password;
                    if(item.Email is not null) cast.Email = item.Email;
                    if(item.Name is not null) cast.Name = item.Name;
                    if(item.Surname is not null) cast.Surname = item.Surname;
                    break;
                }
                case Entities.User: {
                    if(item.Username is not null) user.Username = item.Username;
                    if(item.Password is not null) user.Password = item.Password;
                    if(item.Email is not null) user.Email = item.Email;
                    break;
                }
            }
            
            context.Users.Update(user);
            context.SaveChanges();

            context.Dispose();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            QuickBiteDbContext context = new QuickBiteDbContext();
        
            int loggedUserId = -1;
        
            User loggedUser = context.Users
                .Where(u => u.Id == loggedUserId)
                .FirstOrDefault();

            if (loggedUser == null) return Unauthorized();

            if(loggedUser.Id != id && loggedUser is not Administrator) return Forbid();

            // if we are logged as an administrator we need to find the user we want to update
            User user = loggedUser is Administrator 
            ? context.Users.Where(i => i.Id == id).FirstOrDefault()
            : loggedUser; 

            context.Users.Remove(user);
            context.SaveChanges();

            context.Dispose();

            return Ok(user);
        }
    }
}
