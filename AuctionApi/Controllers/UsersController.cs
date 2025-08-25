using AuctionApi.Models.Users;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _environment;

    public UsersController(IUserService userService, IWebHostEnvironment environment)
    {
        _userService = userService;
        _environment = environment;
    }

    [HttpPost("register")]
    public IActionResult Register([FromForm] RegisterRequest model)
    {
        _userService.Register(model, _environment.WebRootPath);
        return Ok(new { message = "Registration successful" });
    }

    // NEW: Endpoint for seller registration
    [HttpPost("register-seller")]
    public IActionResult RegisterSeller([FromForm] RegisterRequest model)
    {
        _userService.RegisterSeller(model, _environment.WebRootPath);
        return Ok(new { message = "Seller registration successful" });
    }

    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }

    // Example protected endpoint for future dashboard access
    [Authorize(Roles = "Admin")]
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new { message = "Welcome to Admin Dashboard" });
    }
}