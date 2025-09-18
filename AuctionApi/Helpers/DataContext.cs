using AuctionApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AuctionApi.Helpers;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
<<<<<<< Updated upstream
=======
    public DbSet<Product> Product { get; set; }

    public DbSet<Order> Order { get; set; }
>>>>>>> Stashed changes
}