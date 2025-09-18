using Microsoft.AspNetCore.Mvc;
using AuctionApi.Services;
using AuctionApi.Entities;
using AuctionApi.Models.Products;

namespace AuctionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            var products = _productService.GetAll();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductUploadRequest model)
        {
            string filePath = null;

            if (model.Photo != null)
            {
                string uploadFolder = Path.Combine(_environment.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                filePath = Path.Combine(uploadFolder, model.Photo.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }
            }

            var product = new Product
            {
                Name = model.Name,
                LoginUserId = model.LoginUserId,
                Price = model.Price,
                Description = model.Description,
                Photo = filePath != null ? $"Uploads/{model.Photo.FileName}" : null
            };

            var response = _productService.Create(product);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _productService.Delete(id);
            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
