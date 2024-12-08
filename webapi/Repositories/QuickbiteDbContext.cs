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
        // Very secret password wooo
        optionsBuilder
            .UseSqlServer("Server=localhost;Database=quickbite;User Id=sa;Password=Secret123#;");
    }
}

