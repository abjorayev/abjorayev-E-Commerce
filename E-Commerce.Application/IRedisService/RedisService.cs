using Castle.Core.Logging;
using Castle.DynamicProxy.Generators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace E_Commerce.Application.IRedisService
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _distributedCache;
        private ILogger<RedisService> _redisService;

        public RedisService(IDistributedCache distributedCache, ILogger<RedisService> redisService)
        {
            _distributedCache = distributedCache;
            _redisService = redisService;
        }

        public async Task SetDataAsync<T>(string key, T data, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(json);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
            };

            await _distributedCache.SetAsync(key, bytes, options);
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedResponse))
                return JsonConvert.DeserializeObject<T>(cachedResponse);
            return default;
        }

        public async Task DeleteRedisData(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

    }
}
