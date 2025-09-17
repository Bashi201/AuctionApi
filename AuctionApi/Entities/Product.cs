namespace AuctionApi.Entities
{
    public class Product
    {
        public int ProductId { get; set; }   // Primary Key
        public string Name { get; set; }
        public int LoginUserId { get; set; } // Reference to User
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }    // Store file path or URL
    }
}
