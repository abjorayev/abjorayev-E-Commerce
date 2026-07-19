using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.IRedisService
{
    public interface IRedisService
    {
        Task SetDataAsync<T>(string key, T data, TimeSpan? expiry = null);
        Task<T> GetDataAsync<T>(string key);
        Task DeleteRedisData(string key);
    }
}
