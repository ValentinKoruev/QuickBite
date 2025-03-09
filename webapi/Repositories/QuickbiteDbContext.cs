using QuickBite.Entities;
using Microsoft.EntityFrameworkCore;

namespace QuickBite.Repositories;
public class QuickBiteDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<Product> Products{ get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    private int userIndex = -1;
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
        modelBuilder.Entity<User>()
                .HasDiscriminator<int>("UserType")
                .HasValue<User>(0)
                .HasValue<Customer>(1)
                .HasValue<Courier>(2)
                .HasValue<Restaurant>(3)
                .HasValue<Administrator>(9999);
        
        modelBuilder.Entity<Customer>().Property(x => x.Name).HasColumnName("Name");
        modelBuilder.Entity<Customer>().Property(x => x.Surname).HasColumnName("Surname");
        modelBuilder.Entity<Customer>().Property(x => x.Address).HasColumnName("Address");

        modelBuilder.Entity<Courier>().Property(x=> x.Name).HasColumnName("Name");
        modelBuilder.Entity<Courier>().Property(x=> x.Surname).HasColumnName("Surname");

        modelBuilder.Entity<Restaurant>().Property(x=> x.Name).HasColumnName("Name");
        modelBuilder.Entity<Restaurant>().Property(x=> x.Address).HasColumnName("Address");

        modelBuilder.Entity<Administrator>().Property(x=> x.Name).HasColumnName("Name");
        modelBuilder.Entity<Administrator>().Property(x=> x.Surname).HasColumnName("Surname");
        
        modelBuilder.Entity<Customer>().HasData(GetCustomers());
        modelBuilder.Entity<Courier>().HasData(GetCouriers());
        modelBuilder.Entity<Restaurant>().HasData(GetRestaurants());
        modelBuilder.Entity<Administrator>().HasData(GetAdministrators());
        modelBuilder.Entity<Product>().HasData(GetProducts());
    }
    private IEnumerable<Customer> GetCustomers() {
        return [
            new Customer() {
                Id = -1,
                Username = "valentinkoruev5",
                Password = "pass123",
                Email = "valentinkoruev@gmail.com",
                Name = "Valentin",
                Surname = "Koruev"
        }];
    }
    private IEnumerable<Courier> GetCouriers() {
        return [
            new Courier() {
                Id = -2,
                Username = "petar444",
                Password = "pass123",
                Email = "petar444@gmail.com",
                Name = "Petar",
                Surname = "Petrov"
        }];
    }
    private enum RestaurantDummyIds {
        Dominos = -3,
        KFC = -4,
        McDonalds = -5,
    }
    private IEnumerable<Restaurant> GetRestaurants() {
        return [
            new Restaurant() {
                Id = -3,
                Username = "dominos",
                Password = "pass123",
                Email = "takeout@dominos.bg",
                Name = "Domino's Pizza",
                Address = "Sofia, zh.k. Lyulin 6, bul. Pancho Vladigerov 6"
            },
            new Restaurant() {
                Id = -4,
                Username = "kfc",
                Password = "pass123",
                Email = "takeout@kfc.bg",
                Address = "Sofia, zh.k. Lyulin 8, Tsaritsa Yonna 72",
                Name = "KFC"
            },
            new Restaurant() {
                Id = -5,
                Username = "mcdonalds",
                Password = "pass123",
                Email = "takeout@mcdonalds.bg",
                Name = "McDonald's",
                Address = "Sofia, zh.k. Lyulin 10, bul. Evropa 1"
            },
        ];
    }
    private IEnumerable<Administrator> GetAdministrators() {
        return [
            new Administrator() {
                Id = -6,
                Username = "ivanov123",
                Password = "pass123",
                Email = "ivanivanov@gmail.com",
                Name = "Ivan",
                Surname = "Ivanov",
            }
        ];
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

