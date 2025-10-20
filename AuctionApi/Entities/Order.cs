namespace AuctionApi.Entities;

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int BuyerId { get; set; }
    public User Buyer { get; set; }
    public string ShippingAddress { get; set; }
    public string Status { get; set; } // e.g., "Pending", "Shipped", "Delivered"
    public DateTime OrderDate { get; set; }
}