using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.RedisService
{
    public interface ICartRedisService
    {
        Task AddOrUpdateItem(string userId, int productId, int quantity);
        Task<Dictionary<int, int>> GetCart(string userId);
        Task RemoveItem(string userId, int productId);
        Task IncreaseProductCount(string userId, int productId);
        Task DecrementProductCount(string userId, int productId);
    }
}
