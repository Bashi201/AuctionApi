// Updated CreateProductRequest.cs
using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Products;

public class CreateProductRequest
{
    public string? Name { get; set; } // Made nullable
    public string? Description { get; set; } // Made nullable
    public decimal Price { get; set; }
    public string? Status { get; set; } = "Pending"; // Made nullable but with default
    public IFormFile? Image1 { get; set; } // Made nullable
    public IFormFile? Image2 { get; set; } // Made nullable
    public IFormFile? Image3 { get; set; } // Made nullable
    public IFormFile? Image4 { get; set; } // Made nullable
}