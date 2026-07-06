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
        private IDatabase _db;

        public CartRedisService(IDatabase db)
        {
            _db = db;
        }

        public async Task AddOrUpdateItem(int userId, int productId, int quantity)
        {
            await _db.HashSetAsync($"cart:{userId}", productId, quantity);
            await _db.KeyExpireAsync($"cart:{userId}", TimeSpan.FromDays(3));
        }

        public async Task<Dictionary<int, int>> GetCart(int userId)
        {
            var items = await _db.HashGetAllAsync($"cart:{userId}");

            return items.ToDictionary(
                x => (int)x.Name,
                x => (int)x.Value);
        }

        public async Task RemoveItem(int userId, int productId)
        {
            await _db.HashDeleteAsync($"cart:{userId}", productId);
        }
    }
}
