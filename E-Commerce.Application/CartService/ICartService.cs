using E_Commerce.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.CartService
{
    public interface ICartService
    {
        Task<List<CartProductDTO>> GetBasketByUserId(string userId);
        Task AddProduct(string userId, int productId);
        Task IncreaseProductCount(string userId, int productId);
        Task DecrementProductCount(string userId, int productId);
        Task DeleteProductFromBasket(string userId, int productId);
    }
}
