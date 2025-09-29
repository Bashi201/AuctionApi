using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Products;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Services;

public interface IProductService
{
    void CreateProduct(CreateProductRequest model, int sellerId, string rootPath);
    IEnumerable<Product> GetSellerProducts(int sellerId);
    void DeleteProduct(int id, int sellerId);
    void UpdateProduct(int id, UpdateProductResponse model, int sellerId, string rootPath);
    IEnumerable<Product> GetAllProducts();
}

public class ProductService : IProductService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ProductService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void CreateProduct(CreateProductRequest model, int sellerId, string rootPath)
    {
        var product = _mapper.Map<Product>(model);
        product.SellerId = sellerId;

        var images = new List<IFormFile> { model.Image1, model.Image2, model.Image3, model.Image4 };
        foreach (var image in images)
        {
            if (image != null)
            {
                var uniqueFileName = GetUniqueFileName(image.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "AuctionApi", "Images", "products");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                image.CopyTo(new FileStream(filePath, FileMode.Create));
                product.Images.Add("/AuctionApi/Images/products/" + uniqueFileName);
            }
        }

        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public IEnumerable<Product> GetSellerProducts(int sellerId)
    {
        return _context.Products
            .Include(p => p.Seller)
            .Where(p => p.SellerId == sellerId)
            .ToList();
    }

    public void DeleteProduct(int id, int sellerId)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id && p.SellerId == sellerId);
        if (product == null) throw new AppException("Product not found or unauthorized");
        _context.Products.Remove(product);
        _context.SaveChanges();
    }

    private string GetUniqueFileName(string fileName)
    {
        fileName = Path.GetFileName(fileName);
        return Path.GetFileNameWithoutExtension(fileName)
            + "_" + Guid.NewGuid().ToString()[..4]
            + Path.GetExtension(fileName);
    }
    public void UpdateProduct(int id, UpdateProductResponse model, int sellerId, string rootPath)
    {
        var product = _context.Products
            .FirstOrDefault(p => p.Id == id && p.SellerId == sellerId);

        if (product == null)
            throw new AppException("Product not found or unauthorized");

        // Update scalar fields
        product.Name = model.Name ?? product.Name;
        product.Description = model.Description ?? product.Description;
        product.Price = model.Price != 0 ? model.Price : product.Price;

        // Handle images (replace old if new ones are uploaded)
         var images = new List<IFormFile> { model.Image1, model.Image2, model.Image3, model.Image4 };
        if (images.Any(i => i != null))
        {
            product.Images.Clear(); // remove old images

            foreach (var image in images)
            {
                if (image != null)
                {
                    var uniqueFileName = GetUniqueFileName(image.FileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "AuctionApi", "Images", "products");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    image.CopyTo(stream);

                    product.Images.Add("/AuctionApi/Images/products/" + uniqueFileName);
                }
            }
        }

        _context.Products.Update(product);
        _context.SaveChanges();
    }
    //get all



    public IEnumerable<Product> GetAllProducts()
    {
        return _context.Products
    .Include(p => p.Seller) // include seller info too
    .ToList();

    }
}