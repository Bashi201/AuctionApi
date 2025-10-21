using AuctionApi.Entities;
using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Bids;

public class CreateBidRequest
{
    public int AuctionId { get; set; }
    public decimal Amount { get; set; }
}
