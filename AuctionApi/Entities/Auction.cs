namespace AuctionApi.Entities;

public class Auction
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal StartingPrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } // e.g., "Active", "Stopped", "Completed"
    public int SellerId { get; set; }
    public User Seller { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public List<Bid> Bids { get; set; } = new List<Bid>();
    public int? WinnerId { get; set; }
    public User? Winner { get; set; }
}
