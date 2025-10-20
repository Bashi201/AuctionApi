using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Auctions;

public class UpdateAuctionRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal StartingPrice { get; set; }
    public DateTime EndTime { get; set; }
    public IFormFile? Image1 { get; set; }
    public IFormFile? Image2 { get; set; }
    public IFormFile? Image3 { get; set; }
    public IFormFile? Image4 { get; set; }
    public DateTime StartTime { get; internal set; }
}