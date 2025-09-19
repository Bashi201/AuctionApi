using AuctionApi.Models.Sellers;
using AuctionApi.Models.Users;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Seller")] // Restrict all endpoints to Seller role
public class SellersController : ControllerBase
{
    private readonly ISellerService _sellerService;
    private readonly IWebHostEnvironment _environment;

    public SellersController(ISellerService sellerService, IWebHostEnvironment environment)
    {
        _sellerService = sellerService;
        _environment = environment;
    }

    // Register as Admin
    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromForm] SellerRegisterRequestAsSeller model)
    {
        _sellerService.RegisterSeller(model, _environment.ContentRootPath);
        return Ok(new { message = "Registration successful" });
    }

    // Register as Seller
    [HttpPost("register-seller")]
    [AllowAnonymous]
    public IActionResult RegisterSeller([FromForm] SellerRegisterRequestAsSeller model)
    {
        _sellerService.RegisterSeller(model, _environment.ContentRootPath);
        return Ok(new { message = "Seller registration successful" });
    }

    // Authenticate/Login
    [HttpPost("authenticate")]
    [AllowAnonymous]
    public IActionResult Authenticate([FromBody] AuthenticateRequest model)
    {
        var response = _sellerService.Authenticate(model); // Ensure this method exists in SellerService
        return Ok(response);
    }

    // Seller Dashboard
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new { message = "Welcome to Seller Dashboard" });
    }

    // Get all sellers
    [HttpGet]
    public IActionResult GetAllSellers()
    {
        var sellers = _sellerService.GetAllSeller();
        return Ok(sellers);
    }

    // Get seller by ID
    [HttpGet("{id}")]
    public IActionResult GetSellerById(int id)
    {
        var seller = _sellerService.GetSellerById(id);
        if (seller == null)
            return NotFound(new { message = "Seller not found" });
        return Ok(seller);
    }

    // Update seller
    [HttpPut("{id}")]
    public IActionResult UpdateSeller(int id, [FromForm] UpdateSellerRequest model)
    {
        _sellerService.UpdateSeller(id, model, _environment.ContentRootPath);
        return Ok(new { message = "Seller updated successfully" });
    }

    // Delete seller
    [HttpDelete("{id}")]
    public IActionResult DeleteSeller(int id)
    {
        _sellerService.DeleteSeller(id);
        return Ok(new { message = "Seller deleted successfully" });
    }
}
