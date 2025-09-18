using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Products
{
    public class ProductUploadRequest
    {
        public string Name { get; set; }
        public int LoginUserId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        // File upload
        public IFormFile Photo { get; set; }
    }
}
