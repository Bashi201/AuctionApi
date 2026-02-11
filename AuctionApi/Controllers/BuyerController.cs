using AuctionApi.Entities;
using AuctionApi.Models.Auctions;
using AuctionApi.Models.Bids;
using AuctionApi.Models.Orders;
using AuctionApi.Models.Products;
using AuctionApi.Models.Users;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer")]
public class BuyerController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuctionService _auctionService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly IBidService _bidService;
    private readonly IWebHostEnvironment _environment;


    public BuyerController(
        IUserService userService,
        IAuctionService auctionService,
        IProductService productService,
        IOrderService orderService, 
        IBidService bidService,
        IWebHostEnvironment environment
        )
    {
        _userService = userService;
        _auctionService = auctionService;
        _productService = productService;
        _orderService = orderService;
        _bidService = bidService;
        _environment = environment;
    }

    
    [AllowAnonymous]
    [HttpGet("auctions/")]
    public IActionResult GetAllAtuctions()
    {
        var postedauctions = _auctionService.GetAllAuctions()
            .Where(a => string.Equals(a.Status, "posted", StringComparison.OrdinalIgnoreCase)).ToList();
        return Ok(postedauctions);
    }

    [HttpPost("auctions/bid/")]
    public bool AddNewBid([FromForm] CreateBidRequest model)
    {
        User bidder = _userService.GetUserById(int.Parse(User.FindFirst("id")?.Value));
        bool result = _bidService.AddNewBid(model,bidder);

        return (result);
    }

    // New public endpoint for posted products (assuming Product has Status property)
    [AllowAnonymous]
    [HttpGet("products")]
    public IActionResult Products_GetPostedProducts()
    {
        var postedProducts = _productService.GetAllProducts()
            .Where(p => string.Equals(p.Status, "posted", StringComparison.OrdinalIgnoreCase))
            .Select(p => new
            {
                id = p.Id,
                title = p.Name,
                price = p.Price,
                image = p.Images, 
                category = "General" 
            })
            .ToList();

        return Ok(postedProducts);
    }

    [HttpGet("product{id}")]
    [AllowAnonymous]
    public IActionResult Products_GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpGet("auction{id}")]
    [AllowAnonymous]
    public IActionResult Auctions_GetById(int id)
    {
        var auction = _auctionService.GetAuctionByIdAdmin(id);
        if (auction == null) return NotFound(new { message = "auction not found" });
        return Ok(auction);
    }
}
