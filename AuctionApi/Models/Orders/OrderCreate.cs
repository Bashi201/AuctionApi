using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Orders
{
    public class OrderCreate
    {
        public string ShippingAddress { get; set; }
        public DateTime Date { get; set; }
        public string ContactNumber { get; set; }
        public int Users_Id { get; set; }
    }
}