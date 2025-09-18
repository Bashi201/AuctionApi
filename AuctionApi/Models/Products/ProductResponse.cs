namespace AuctionApi.Models.Products
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int LoginUserId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; } // URL or file path
    }
}
