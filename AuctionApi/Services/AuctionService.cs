using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Auctions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Services;

public interface IAuctionService
{
    void CreateAuction(CreateAuctionRequest model, int sellerId, string rootPath);
    IEnumerable<Auction> GetSellerAuctions(int sellerId);
    Auction GetAuctionById(int id, int sellerId);
    void DeleteAuction(int id, int sellerId);
    void ExtendAuctionTime(int id, int sellerId, int additionalHours);
    void StopAuction(int id, int sellerId);
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
        auction.Status = "Active";

        var customBasePath = Path.Combine(Directory.GetCurrentDirectory(), "AuctionApi", "images", "auctions");
        var uploadsFolder = customBasePath;
        Directory.CreateDirectory(uploadsFolder);

        var images = new List<IFormFile> { model.Image1, model.Image2, model.Image3, model.Image4 };
        foreach (var image in images)
        {
            if (image != null)
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
}