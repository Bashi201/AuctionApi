using AuctionApi.Helpers;
using AuctionApi.Models.Auctions;
using AuctionApi.Models.Orders;
using AuctionApi.Models.Products;
using AuctionApi.Models.Users;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Seller")]
public class SellersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuctionService _auctionService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly IWebHostEnvironment _environment;

    public SellersController(
        IUserService userService,
        IAuctionService auctionService,
        IProductService productService,
        IOrderService orderService,
        IWebHostEnvironment environment)
    {
        _userService = userService;
        _auctionService = auctionService;
        _productService = productService;
        _orderService = orderService;
        _environment = environment;
    }

    // Auction Management: Create a new auction
    [HttpPost("auctions")]
    public IActionResult CreateAuction([FromForm] CreateAuctionRequest model)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _auctionService.CreateAuction(model, sellerId, _environment.WebRootPath);
        return Ok(new { message = "Auction created successfully" });
    }

    // Auction Management: View all auctions created by the seller
    [HttpGet("auctions")]
    public IActionResult GetSellerAuctions()
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        var auctions = _auctionService.GetSellerAuctions(sellerId);
        return Ok(auctions);
    }

    // Auction Management: View specific auction details (including bidders and winner)
    [HttpGet("auctions/{id}")]
    public IActionResult GetAuctionById(int id)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        var auction = _auctionService.GetAuctionById(id, sellerId);
        if (auction == null)
            return NotFound(new { message = "Auction not found or unauthorized" });
        return Ok(auction);
    }

    // Auction Management: Delete an auction
    [HttpDelete("auctions/{id}")]
    public IActionResult DeleteAuction(int id)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _auctionService.DeleteAuction(id, sellerId);
        return Ok(new { message = "Auction deleted successfully" });
    }

    // Auction Management: Extend auction time
    [HttpPut("auctions/{id}/extend")]
    public IActionResult ExtendAuctionTime(int id, [FromBody] ExtendAuctionTimeRequest model)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _auctionService.ExtendAuctionTime(id, sellerId, model.AdditionalHours);
        return Ok(new { message = "Auction time extended successfully" });
    }

    [HttpPut("auctions/{id}")]
    public IActionResult UpdateAuction(int id, [FromForm] UpdateAuctionRequest model)
    {
        try
        {
            var sellerId = int.Parse(User.FindFirst("id")?.Value);
            _auctionService.UpdateAuction(id, model, sellerId, _environment.WebRootPath);
            return Ok(new { message = "Auction updated successfully" });
        }
        catch (AppException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Auction Management: Stop auction
    [HttpPut("auctions/{id}/stop")]
    public IActionResult StopAuction(int id)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _auctionService.StopAuction(id, sellerId);
        return Ok(new { message = "Auction stopped successfully" });
    }

    // Product Management: Create a new product
    [HttpPost("products")]
    public IActionResult CreateProduct([FromForm] CreateProductRequest model)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _productService.CreateProduct(model, sellerId, _environment.WebRootPath);
        return Ok(new { message = "Product created successfully" });
    }

    // Product Management: Update a product
    [HttpPut("products/{id}")]
    public IActionResult UpdateProduct(int id, [FromForm] UpdateProductRequest model)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _productService.UpdateProduct(id, model, sellerId, _environment.WebRootPath);
        return Ok(new { message = "Product updated successfully" });
    }

    // Product Management: View all products created by the seller
    [HttpGet("products")]
    public IActionResult GetSellerProducts()
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        var products = _productService.GetSellerProducts(sellerId);
        return Ok(products);
    }

    // Product Management: Delete a product
    [HttpDelete("products/{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _productService.DeleteProduct(id, sellerId);
        return Ok(new { message = "Product deleted successfully" });
    }

    // Order Management: View all orders for seller's products
    [HttpGet("orders")]
    public IActionResult GetSellerOrders()
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        var orders = _orderService.GetSellerOrders(sellerId);
        return Ok(orders);
    }

    // Order Management: Update order delivery status
    [HttpPut("orders/{id}/status")]
    public IActionResult UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest model)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _orderService.UpdateOrderStatus(id, sellerId, model.Status);
        return Ok(new { message = "Order status updated successfully" });
    }

    // Account Management: Get seller account details
    [HttpGet("account")]
    public IActionResult GetAccount()
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        var user = _userService.GetUserById(sellerId);
        return Ok(user);
    }

    // Account Management: Update seller account details
    [HttpPut("account")]
    public IActionResult UpdateAccount([FromForm] UpdateUserRequest model)
    {
        var sellerId = int.Parse(User.FindFirst("id")?.Value);
        _userService.UpdateUser(sellerId, model, _environment.WebRootPath);
        return Ok(new { message = "Account updated successfully" });
    }
   
}