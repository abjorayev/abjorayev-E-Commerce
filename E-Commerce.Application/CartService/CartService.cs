using E_Commerce.Application.DTO;
using E_Commerce.Application.RedisService;
using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartRedisService _redisService;
        private readonly IECommerceRepository<Product> _productRepository;

        public CartService(ICartRedisService redisService, IECommerceRepository<Product> productRepository)
        {
            _redisService = redisService;
            _productRepository = productRepository;
        }

        public async Task<List<CartProductDTO>> GetBasketByUserId(string userId)
        {
            var redisBasket = await _redisService.GetCart(userId);
            var products = await _productRepository.Query().Where(x => redisBasket.Keys.Contains(x.Id) && x.Active).ToListAsync();
            return products.Select(x => new CartProductDTO
            {
                ProductId = x.Id,
                ProductName = x.Name,
                ProductPhoto = x.ImageUrl,
                ProductQuantity = redisBasket[x.Id],
                ProductPrice = redisBasket[x.Id] * x.Price,
            }).ToList();
        }

        public async Task AddProduct(string userId, int productId)
        {
            var product = await _productRepository.Query().FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null || product.ProductCount == 0)
                throw new Exception("Product is null or empty");
            await _redisService.AddOrUpdateItem(userId, productId, 1);
        }

        public async Task IncreaseProductCount(string userId, int productId)
        {
            await _redisService.IncreaseProductCount(userId, productId);
        }

        public async Task DecrementProductCount(string userId, int productId)
        {
            await _redisService.DecrementProductCount(userId, productId);
        }

        public async Task DeleteProductFromBasket(string userId, int productId)
        {
            await _redisService.RemoveItem(userId, productId);
        }

        public async Task DeleteBasket(string userId)
        {
            await _redisService.Delete(userId);
        }
    }
}
