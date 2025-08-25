using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Users;

public class RegisterRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}