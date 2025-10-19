using AuctionApi.Models.Products;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(IProductService productService, IWebHostEnvironment environment)
    {
        _productService = productService;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var products = _productService.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromForm] UpdateProductRequest model)
    {
        _productService.UpdateProductAdmin(id, model, _environment.WebRootPath);
        return Ok(new { message = "Product updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _productService.DeleteProductAdmin(id);
        return Ok(new { message = "Product deleted successfully" });
    }

    // New public endpoint for posted products (assuming Product has Status property)
    [AllowAnonymous]
    [HttpGet("posted")]
    public IActionResult GetPostedProducts()
    {
        var postedProducts = _productService.GetAllProducts()
            .Where(p => string.Equals(p.Status, "posted", StringComparison.OrdinalIgnoreCase))
            .Select(p => new
            {
                id = p.Id,
                title = p.Name, // Map to 'title' to match frontend usage
                price = p.Price,
                image = p.Images, // Use directly as src, assuming "/AuctionApi/..." path
                category = "General" // Default category since no column in DB
            })
            .ToList();

        return Ok(postedProducts);
    }
}