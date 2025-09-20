using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Orders;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Services;

public interface IOrderService
{
    IEnumerable<Order> GetSellerOrders(int sellerId);
    void UpdateOrderStatus(int id, int sellerId, string status);
}

public class OrderService : IOrderService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public OrderService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IEnumerable<Order> GetSellerOrders(int sellerId)
    {
        return _context.Order
            .Include(o => o.Product)
            .Include(o => o.Buyer)
            .Where(o => o.Product.SellerId == sellerId)
            .ToList();
    }

    public void UpdateOrderStatus(int id, int sellerId, string status)
    {
        var order = _context.Order
            .Include(o => o.Product)
            .FirstOrDefault(o => o.Id == id && o.Product.SellerId == sellerId);
        if (order == null) throw new AppException("Order not found or unauthorized");
        order.Status = status;
        _context.SaveChanges();
    }
}