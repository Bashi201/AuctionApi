using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Auctions;
using AuctionApi.Models.Products;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Services;

public interface IAuctionService
{
    void CreateAuction(CreateAuctionRequest model, int sellerId, string rootPath);
    void UpdateAuction(int id, UpdateAuctionRequest model, int sellerId, string rootPath); // New
    IEnumerable<Auction> GetAllAuctions();
    IEnumerable<Auction> GetSellerAuctions(int sellerId);
    Auction GetAuctionById(int id, int sellerId);
    void DeleteAuction(int id, int sellerId);
    void ExtendAuctionTime(int id, int sellerId, int additionalHours);
    void StopAuction(int id, int sellerId);

    Auction GetAuctionByIdAdmin(int id);
    void UpdateAuctionAdmin(int id, UpdateAuctionRequest model, string rootPath);
    void DeleteAuctionAdmin(int id);
}

public class AuctionService : IAuctionService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public AuctionService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public void CreateAuction(CreateAuctionRequest model, int sellerId, string rootPath)
    {
        var auction = _mapper.Map<Auction>(model);
        auction.SellerId = sellerId;
        auction.StartTime = DateTime.UtcNow;
        auction.Status = "Pending";

        var customBasePath = Path.Combine(Directory.GetCurrentDirectory(), "AuctionApi", "images", "auctions");
        var uploadsFolder = customBasePath;
        Directory.CreateDirectory(uploadsFolder);

        var images = new List<IFormFile?> { model.Image1, model.Image2, model.Image3, model.Image4 };  // Made nullable for consistency
        foreach (var image in images)
        {
            if (image != null && image.Length > 0)  // Added Length check for safety
            {
                var uniqueFileName = GetUniqueFileName(image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                image.CopyTo(new FileStream(filePath, FileMode.Create));
                auction.Images.Add("/AuctionApi/images/auctions/" + uniqueFileName);
            }
        }

        _context.Auctions.Add(auction);
        _context.SaveChanges();
    }

    public void UpdateAuction(int id, UpdateAuctionRequest model, int sellerId, string rootPath)
    {
        var auction = _context.Auctions.FirstOrDefault(a => a.Id == id && a.SellerId == sellerId);
        if (auction == null) throw new AppException("Auction not found or unauthorized");

        // Update core fields (unconditional, as they're required in the model)
        auction.Title = model.Title;
        auction.Description = model.Description;
        auction.StartingPrice = model.StartingPrice;
        auction.EndTime = model.EndTime;

        // Handle image updates (replace all images only if new ones are provided, like in UpdateProduct)
        var newImages = new List<IFormFile?> { model.Image1, model.Image2, model.Image3, model.Image4 };
        if (newImages.Any(img => img != null && img.Length > 0))
        {
            auction.Images.Clear(); // Clear existing images
            var customBasePath = Path.Combine(Directory.GetCurrentDirectory(), "AuctionApi", "images", "auctions");
            var uploadsFolder = customBasePath;
            Directory.CreateDirectory(uploadsFolder);
            foreach (var image in newImages)
            {
                if (image != null && image.Length > 0)
                {
                    var uniqueFileName = GetUniqueFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    image.CopyTo(new FileStream(filePath, FileMode.Create));
                    auction.Images.Add("/AuctionApi/images/auctions/" + uniqueFileName);
                }
            }
        }

        _context.SaveChanges();  // No explicit Update() needed, as entity is tracked
    }

    public IEnumerable<Auction> GetSellerAuctions(int sellerId)
    {
        return _context.Auctions
            .Include(a => a.Seller)
            .Include(a => a.Bids)
            .ThenInclude(b => b.Bidder)
            .Include(a => a.Winner)
            .Where(a => a.SellerId == sellerId)
            .ToList();
    }
    public IEnumerable<Auction> GetAllAuctions()
    {
        return _context.Auctions
            .Include(a => a.Seller)
            .Include(a => a.Bids)
            .ThenInclude(b => b.Bidder)
            .Include(a => a.Winner)
            .ToList();
    }
    public Auction GetAuctionById(int id, int sellerId)
    {
        var auction = _context.Auctions
            .Include(a => a.Seller)
            .Include(a => a.Bids)
            .ThenInclude(b => b.Bidder)
            .Include(a => a.Winner)
            .FirstOrDefault(a => a.Id == id && a.SellerId == sellerId);
        if (auction == null) throw new AppException("Auction not found or unauthorized");
        return auction;
    }

    public Auction GetAuctionByIdAdmin(int id)
    {
        var auction = _context.Auctions
            .Include(a => a.Seller)
            .Include(a => a.Bids)
            .ThenInclude(b => b.Bidder)
            .Include(a => a.Winner)
            .FirstOrDefault(a => a.Id == id);
        if (auction == null) throw new AppException("Auction not found or unauthorized");
        return auction;
    }
    public void DeleteAuction(int id, int sellerId)
    {
        var auction = _context.Auctions.FirstOrDefault(a => a.Id == id && a.SellerId == sellerId);
        if (auction == null) throw new AppException("Auction not found or unauthorized");
        _context.Auctions.Remove(auction);
        _context.SaveChanges();
    }

    public void ExtendAuctionTime(int id, int sellerId, int additionalHours)
    {
        var auction = _context.Auctions.FirstOrDefault(a => a.Id == id && a.SellerId == sellerId);
        if (auction == null) throw new AppException("Auction not found or unauthorized");
        if (auction.Status != "Active") throw new AppException("Cannot extend a non-active auction");
        auction.EndTime = auction.EndTime.AddHours(additionalHours);
        _context.SaveChanges();
    }

    public void StopAuction(int id, int sellerId)
    {
        var auction = _context.Auctions.FirstOrDefault(a => a.Id == id && a.SellerId == sellerId);
        if (auction == null) throw new AppException("Auction not found or unauthorized");
        if (auction.Status != "Active") throw new AppException("Cannot stop a non-active auction");
        auction.Status = "Stopped";
        _context.SaveChanges();
    }

    private string GetUniqueFileName(string fileName)
    {
        fileName = Path.GetFileName(fileName);
        return Path.GetFileNameWithoutExtension(fileName)
            + "_" + Guid.NewGuid().ToString()[..4]
            + Path.GetExtension(fileName);
    }



    public void DeleteAuctionAdmin(int id)
    {
        var auction = GetAuctionByIdAdmin(id);
        if (auction == null) throw new AppException("Auction not found");
        _context.Auctions.Remove(auction);
        _context.SaveChanges();
    }

    public void UpdateAuctionAdmin(int id, UpdateAuctionRequest model, string rootPath)
    {
        var auction = GetAuctionByIdAdmin(id);
        if (auction == null) throw new AppException("auction not found");

        if (!string.IsNullOrEmpty(model.Title)) auction.Title = model.Title;
        if (!string.IsNullOrEmpty(model.Description)) auction.Description = model.Description;
        if (!model.StartingPrice.Equals(0)) auction.StartingPrice = model.StartingPrice;
        if (!string.IsNullOrEmpty(model.Status)) auction.Status = model.Status;
        if (!model.EndTime.Equals(null)) auction.EndTime = model.EndTime;

        var newImages = new List<IFormFile> { model.Image1, model.Image2, model.Image3, model.Image4 };
        if (newImages.Any(img => img != null))
        {
            auction.Images.Clear();
            foreach (var image in newImages)
            {
                if (image != null)
                {
                    var uniqueFileName = GetUniqueFileName(image.FileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "AuctionApi", "Images", "auctions");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }
                    auction.Images.Add("/AuctionApi/Images/auctions/" + uniqueFileName);
                }
            }
        }

        _context.Auctions.Update(auction);
        _context.SaveChanges();
    }
}
