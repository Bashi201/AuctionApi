using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Sellers
{
    public class SellerRegisterRequestAsSeller
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}

