using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Users;

public class SellerRegisterRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public string Role { get; set; } = "Seller";
}