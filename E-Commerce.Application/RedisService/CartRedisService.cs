using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.RedisService
{
    public class CartRedisService : ICartRedisService
    {
        private readonly IDatabase _db;

        public CartRedisService(IConnectionMultiplexer multiplexer)
        {
            _db = multiplexer.GetDatabase();
        }

        public async Task AddOrUpdateItem(string userId, int productId, int quantity)
        {
            await _db.HashSetAsync($"cart:{userId}", productId, quantity);
            await _db.KeyExpireAsync($"cart:{userId}", TimeSpan.FromDays(3));
        }

        public async Task IncreaseProductCount(string userId, int productId)
        {
            await _db.HashIncrementAsync($"cart:{userId}", productId, 1);
            await _db.KeyExpireAsync($"cart:{userId}", TimeSpan.FromDays(3));
        }

        public async Task DecrementProductCount(string userId, int productId)
        {
            await _db.HashDecrementAsync($"cart:{userId}", productId, 1);
            await _db.KeyExpireAsync($"cart:{userId}", TimeSpan.FromDays(3));
        }

        public async Task<Dictionary<int, int>> GetCart(string userId)
        {
            var items = await _db.HashGetAllAsync($"cart:{userId}");

            return items.ToDictionary(
                x => (int)x.Name,
                x => (int)x.Value);
        }

        public async Task RemoveItem(string userId, int productId)
        {
            await _db.HashDeleteAsync($"cart:{userId}", productId);
        }
    }
}
