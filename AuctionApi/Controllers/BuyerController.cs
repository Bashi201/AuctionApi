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

    // Order Management: View all orders placed by buyers for the seller's products
    [AllowAnonymous]
    [HttpGet("auctions/")]
    public IActionResult GetAllAtuctions()
    {
        var auctions = _auctionService.GetAllAuctions();
        return Ok(auctions);
    }

    [HttpPost("auctions/bid/")]
    public bool AddNewBid([FromForm] CreateBidRequest model)
    {
        User bidder = _userService.GetUserById(int.Parse(User.FindFirst("id")?.Value));
        bool result = _bidService.AddNewBid(model,bidder);

        return (result);
    }

}
