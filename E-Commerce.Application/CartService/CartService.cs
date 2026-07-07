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

        public async Task<List<CartProductDTO>> GetBasketByUserId(int userId)
        {
            var redisBasket = await _redisService.GetCart(userId);
            var products = await _productRepository.Query().Where(x => redisBasket.Keys.Contains(x.Id) && x.Active).ToListAsync();
            return products.Select(x => new CartProductDTO
            {
                ProductId = x.Id,
                ProductName = x.Name,
                ProductPhoto = x.ImageUrl,
                ProductQuantity = redisBasket[x.Id]
            }).ToList();
        }

        public async Task AddProduct(int userId, int productId)
        {
            await _redisService.AddOrUpdateItem(userId, productId, 1);
        }

        public async Task IncreaseProductCount(int userId, int productId)
        {
            await _redisService.IncreaseProductCount(userId, productId);
        }

        public async Task DecrementProductCount(int userId, int productId)
        {
            await _redisService.DecrementProductCount(userId, productId);
        }

        public async Task DeleteProductFromBasket(int userId, int productId)
        {
            await _redisService.RemoveItem(userId, productId);
        }
    }
}
