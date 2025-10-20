// Updated Product.cs
namespace AuctionApi.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int SellerId { get; set; }
    public User? Seller { get; set; } // Made nullable
    public List<string> Images { get; set; } = new List<string>();
    public string Status { get; set; } = "Pending";

    // Constructor to ensure initialization
    public Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        Status = "Pending";
        Images = new List<string>();
    }
}