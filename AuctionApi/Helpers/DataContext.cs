using AuctionApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AuctionApi.Helpers;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Auction> Auctions { get; set; }
    public DbSet<Bid> Bids { get; set; }
}