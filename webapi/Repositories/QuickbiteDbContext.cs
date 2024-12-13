using QuickBite.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickBite.Repositories;
public class QuickBiteDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Product> Products{ get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }

    public QuickBiteDbContext()
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Top secret password wooo
        optionsBuilder
            .UseSqlServer("Server=localhost;Database=quickbite;User Id=sa;Password=Secret123#;TrustServerCertificate=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(GetUsers());
        modelBuilder.Entity<Restaurant>().HasData(GetRestaurants());
        modelBuilder.Entity<Product>().HasData(GetProducts());
    }

    private IEnumerable<User> GetUsers() {
        int index = -1;
        User[] users =
        [
            new User() {
                Id = index--,
                Username = "valentinkoruev5",
                Password = "pass123",
                Email = "valentinkoruev@gmail.com",
                FirstName = "Valentin",
                LastName = "Koruev",
                Type = User.UserType.Customer
            },
            new User() {
                Id = index--,
                Username = "petar444",
                Password = "pass123",
                Email = "petar444@gmail.com",
                FirstName = "Petar",
                LastName = "Petrov",
                Type = User.UserType.Courier
            },
            new User() {
                Id = index--,
                Username = "ivanov123",
                Password = "pass123",
                Email = "ivanivanov@gmail.com",
                FirstName = "Ivan",
                LastName = "Ivanov",
                Type = User.UserType.Administrator
            },
        ];

        return users;
    }
    private IEnumerable<Restaurant> GetRestaurants() 
    {
        int index = -1;
        Restaurant[] restaurants = [
            new Restaurant() {
                Id = index--,
                Name = "Domino's Pizza",
                Address = "Sofia, zh.k. Lyulin 6, bul. Pancho Vladigerov 6"
            },
            new Restaurant() {
                Id = index--,
                Name = "KFC",
                Address = "Sofia, zh.k. Lyulin 8, bul. Tsaritsa Yoanna 72"
            },
            new Restaurant() {
                Id = index--,
                Name = "McDonald's",
                Address = "Sofia, zh.k. Lulin 10, bul. Evropa 1"
            }
        ];

        return restaurants;
    }

    private enum RestaurantDummyIds {
        Dominos = -1,
        KFC = -2,
        McDonalds = -3,
    }
    private IEnumerable<Product> GetProducts() {
        int index = -1;
        Product[] products = [
            new Product() {
                Id = index--,
                Name = "Master Burger Pizza",
                Price = 15,
                RestaurantId = (int)RestaurantDummyIds.Dominos
            },
            new Product() {
                Id = index--,
                Name = "Chik Chirik Pizza",
                Price = 12,
                RestaurantId = (int)RestaurantDummyIds.Dominos
            },
            new Product() {
                Id = index--,
                Name = "Vegetarian Pizza",
                Price = 8,
                RestaurantId = (int)RestaurantDummyIds.Dominos
            },
            new Product() {
                Id = index--,
                Name = "Zinger Burger",
                Price = 10,
                RestaurantId = (int)RestaurantDummyIds.KFC
            },
            new Product() {
                Id = index--,
                Name = "Bonburger",
                Price = 5,
                RestaurantId = (int)RestaurantDummyIds.KFC
            },
            new Product() {
                Id = index--,
                Name = "Bucket 30 Hot",
                Price = 25,
                RestaurantId = (int)RestaurantDummyIds.KFC
            },
            new Product() {
                Id = index--,
                Name = "McCrispy",
                Price = 10,
                RestaurantId = (int)RestaurantDummyIds.McDonalds
            },
            new Product() {
                Id = index--,
                Name = "McNuggets",
                Price = 8,
                RestaurantId = (int)RestaurantDummyIds.McDonalds
            },
            new Product() {
                Id = index--,
                Name = "Hamburger",
                Price = 8,
                RestaurantId = (int)RestaurantDummyIds.McDonalds
            }
        ];

        return products;
    }
}

