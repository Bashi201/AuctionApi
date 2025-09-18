using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Products;

namespace AuctionApi.Services
{
    public interface IProductService
    {
        IEnumerable<ProductResponse> GetAll();
        ProductResponse GetById(int id);
        ProductResponse Create(Product product);
        void Delete(int id);
    }

    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductResponse> GetAll()
        {
            return _context.Product
                .Select(p => new ProductResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    LoginUserId = p.LoginUserId,
                    Price = p.Price,
                    Description = p.Description,
                    Photo = p.Photo
                }).ToList();
        }

        public ProductResponse GetById(int id)
        {
            var product = _context.Product.Find(id);
            if (product == null) return null;

            return new ProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                LoginUserId = product.LoginUserId,
                Price = product.Price,
                Description = product.Description,
                Photo = product.Photo
            };
        }

        public ProductResponse Create(Product product)
        {
            _context.Product.Add(product);
            _context.SaveChanges();

            return new ProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                LoginUserId = product.LoginUserId,
                Price = product.Price,
                Description = product.Description,
                Photo = product.Photo
            };
        }

        public void Delete(int id)
        {
            var product = _context.Product.Find(id);
            if (product == null) throw new KeyNotFoundException("Product not found");

            _context.Product.Remove(product);
            _context.SaveChanges();
        }
    }
}
