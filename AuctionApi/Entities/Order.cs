namespace AuctionApi.Entities
{
    public class Order
    {
        public int OrderId { get; set; }      // Primary Key
        public string ShippingAddress { get; set; }
        public DateTime Date { get; set; }
        public string ContactNumber { get; set; }
        public int Users_Id { get; set; }     // Reference to User
    }
}