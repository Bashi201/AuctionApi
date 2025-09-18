using AuctionApi.Entities;
using AuctionApi.Models.Orders;
using AuctionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _OrderService;
        private readonly IWebHostEnvironment _environment;

        public OrdersController(IOrderService Orderservice, IWebHostEnvironment environment)
        {
            _OrderService = Orderservice;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var Orders = _OrderService.GetAll();
            return Ok(Orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _OrderService.GetById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost("create")]
        public IActionResult Create([FromForm] OrderCreate model)
        {
            // Create a new Order entity from the model
            var orderEntity = new Order
            {
                ShippingAddress = model.ShippingAddress,
                Date = model.Date,
                ContactNumber = model.ContactNumber,
                Users_Id = model.Users_Id
            };

            _OrderService.Create(model, orderEntity);
            return Ok(new { message = "Order Created" });
        }

        [HttpPost("update")]
        public IActionResult Update([FromForm] OrderUpdate model)
        {
            // Create a new Order entity from the model
            var orderEntity = new Order
            {
                OrderId = model.OrderId,
                ShippingAddress = model.ShippingAddress,
                ContactNumber = model.ContactNumber,
            };
            _OrderService.Update(model);
            return Ok(new { message = "Update successful" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _OrderService.Delete(id);
            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
