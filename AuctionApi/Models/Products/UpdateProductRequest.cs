using Microsoft.AspNetCore.Http;

namespace AuctionApi.Models.Products;

public class UpdateProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal? Price { get; set; }
    public string Status { get; set; }
    public IFormFile? Image1 { get; set; }
    public IFormFile? Image2 { get; set; }
    public IFormFile? Image3 { get; set; }
    public IFormFile? Image4 { get; set; }
}