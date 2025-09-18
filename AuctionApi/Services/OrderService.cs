using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Orders;

namespace AuctionApi.Services
{
    public interface IOrderService
    {
        IEnumerable<OrderResponse> GetAll();
        OrderResponse GetById(int id);
        OrderResponse Create(OrderCreate model, Order Order);
        OrderResponse Update(OrderUpdate model);
        void Delete(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly DataContext _context;

        public OrderService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<OrderResponse> GetAll()
        {
            return _context.Order
                .Select(p => new OrderResponse
                {
                    OrderId = p.OrderId,
                    ShippingAddress = p.ShippingAddress,
                    Date = p.Date,
                    ContactNumber = p.ContactNumber,
                    Users_Id = p.Users_Id
                }).ToList();
        }

        public OrderResponse GetById(int id)
        {
            var Order = _context.Order.Find(id);
            if (Order == null) return null;

            return new OrderResponse
            {
                OrderId = Order.OrderId,
                ShippingAddress = Order.ShippingAddress,
                Date = Order.Date,
                ContactNumber = Order.ContactNumber,
                Users_Id = Order.Users_Id
            };
        }

        public OrderResponse Create(OrderCreate model, Order Order)
        {
            _context.Order.Add(Order);
            _context.SaveChanges();

            return new OrderResponse
            {
                OrderId = Order.OrderId,
                ShippingAddress = Order.ShippingAddress,
                Date = Order.Date,
                ContactNumber = Order.ContactNumber,
                Users_Id = Order.Users_Id
            };
        }

        public OrderResponse Update(OrderUpdate model)
        {
            var Order = _context.Order.Find(model.OrderId);
            if (Order == null) throw new KeyNotFoundException("Order not found");
            // Update fields
            Order.ShippingAddress = model.ShippingAddress;
            Order.ContactNumber = model.ContactNumber;
            _context.Order.Update(Order);
            _context.SaveChanges();
            return new OrderResponse
            {
                OrderId = Order.OrderId,
                ShippingAddress = Order.ShippingAddress,
                Date = Order.Date,
                ContactNumber = Order.ContactNumber,
                Users_Id = Order.Users_Id
            };
        }

        public void Delete(int id)
        {
            var Order = _context.Order.Find(id);
            if (Order == null) throw new KeyNotFoundException("Order not found");

            _context.Order.Remove(Order);
            _context.SaveChanges();
        }
    }
}
