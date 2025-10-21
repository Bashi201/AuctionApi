using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Auctions;
using AuctionApi.Models.Bids;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Services;

public interface IBidService
{

    bool AddNewBid(CreateBidRequest model, User bidder);
}

public class BidService : IBidService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public BidService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public bool AddNewBid(CreateBidRequest model, User bidder)
    {
        var bid = _mapper.Map<Bid>(model);
        bid.Amount = model.Amount;
        bid.AuctionId = model.AuctionId;
        bid.BidderId = bidder.Id;
        bid.Bidder = bidder;
        bid.BidTime = DateTime.UtcNow;

        _context.Bids.Add(bid);
        int changes = _context.SaveChanges();
        return changes > 0;
    }
}