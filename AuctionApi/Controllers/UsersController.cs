using AuctionApi.Models.Users;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Restrict all endpoints to Admin role
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _environment;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;

    public UsersController(IUserService userService, IWebHostEnvironment environment, IOrderService orderService, IProductService productService)
    {
        _userService = userService;
        _environment = environment;
        _orderService = orderService;
        _productService = productService;
    }

    [HttpPost("register")]
    [AllowAnonymous] // Allow public access for registration
    public IActionResult Register([FromForm] RegisterRequest model)
    {
        _userService.Register(model, _environment.WebRootPath);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("register-seller")]
    [AllowAnonymous] // Allow public access for seller registration
    public IActionResult RegisterSeller([FromForm] RegisterRequest model)
    {
        _userService.RegisterSeller(model, _environment.WebRootPath);
        return Ok(new { message = "Seller registration successful" });
    }

    [HttpPost("register-buyer")]
    [AllowAnonymous] // Allow public access for buyer registration
    public IActionResult RegisterBuyer([FromForm] RegisterRequest model)
    {
        _userService.RegisterBuyer(model, _environment.WebRootPath);
        return Ok(new { message = "Buyer registration successful" });
    }

    [HttpPost("authenticate")]
    [AllowAnonymous] // Allow public access for authentication
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new { message = "Welcome to Admin Dashboard" });
    }

    // NEW: Get all users
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }

    // NEW: Get user by ID
    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null)
            return NotFound(new { message = "User not found" });
        return Ok(user);
    }

    // NEW: Update user
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromForm] UpdateUserRequest model)
    {
        _userService.UpdateUser(id, model, _environment.WebRootPath);
        return Ok(new { message = "User updated successfully" });
    }

    // NEW: Delete user
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        _userService.DeleteUser(id);
        return Ok(new { message = "User deleted successfully" });
    }

    //shoing order and product
    // Order Management: View all orders for seller's products
    [HttpGet("orders")]
    public IActionResult GetSellerOrders()
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        var orders = _orderService.GetSellerOrders(sellerId);
        return Ok(orders);
    }



    // Product Management: View all products created by the seller
    [HttpGet("products")]
    public IActionResult GetAllProducts()
    {
        var products = _productService.GetAllProducts();  // assign to 'products'
        return Ok(products);
    }
}