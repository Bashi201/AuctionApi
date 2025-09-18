using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Orders
{
    public class OrderUpdate
    {
        public int OrderId { get; set; }
        public string ShippingAddress { get; set; }
        public string ContactNumber { get; set; }
    }
}