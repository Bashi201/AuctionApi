namespace AuctionApi.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int SellerId { get; set; }
    public User Seller { get; set; }
    public List<string> Images  { get; set; } = new List<string>();
}