using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Models.Orders
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime Date { get; set; }
        public string ContactNumber { get; set; }
        public int Users_Id { get; set; }
    }
}
