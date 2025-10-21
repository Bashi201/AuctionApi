using AuctionApi.Entities;
using AuctionApi.Models.Auctions;
using AuctionApi.Models.Products;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IAuctionService _auctionService;
    private readonly IWebHostEnvironment _environment;

    public AdminController(IProductService productService,IAuctionService auctionService, IWebHostEnvironment environment)
    {
        _productService = productService;
        _auctionService = auctionService;
        _environment = environment;
    }

    [HttpGet("product")]
    public IActionResult Products_GetAll()
    {
        var products = _productService.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("product{id}")]
    [AllowAnonymous]
    public IActionResult Products_GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpPut("product{id}")]
    public IActionResult Products_Update(int id, [FromForm] UpdateProductRequest model)
    {
        _productService.UpdateProductAdmin(id, model, _environment.WebRootPath);
        return Ok(new { message = "Product updated successfully" });
    }

    [HttpDelete("product{id}")]
    public IActionResult Products_Delete(int id)
    {
        _productService.DeleteProductAdmin(id);
        return Ok(new { message = "Product deleted successfully" });
    }

    //auctions

    [HttpGet("auction")]
    public IActionResult Auctions_GetAll()
    {
        var auctions = _auctionService.GetAllAuctions();
        return Ok(auctions);
    }

    [HttpGet("auction{id}")]
    [AllowAnonymous]
    public IActionResult Auctions_GetById(int id)
    {
        var auction = _auctionService.GetAuctionByIdAdmin(id);
        if (auction == null) return NotFound(new { message = "auction not found" });
        return Ok(auction);
    }

    [HttpPut("auction{id}")]
    public IActionResult Auctions_Update(int id, [FromForm] UpdateAuctionRequest model)
    {
        _auctionService.UpdateAuctionAdmin(id, model, _environment.WebRootPath);
        return Ok(new { message = "auction updated successfully" });
    }

    [HttpDelete("auction{id}")]
    public IActionResult Auctions_Delete(int id)
    {
        _auctionService.DeleteAuctionAdmin(id);
        return Ok(new { message = "auction deleted successfully" });
    }

}