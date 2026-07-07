using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.RedisService
{
    public interface ICartRedisService
    {
        Task AddOrUpdateItem(int userId, int productId, int quantity);
        Task<Dictionary<int, int>> GetCart(int userId);
        Task RemoveItem(int userId, int productId);
        Task IncreaseProductCount(int userId, int productId);
        Task DecrementProductCount(int userId, int productId);
    }
}
