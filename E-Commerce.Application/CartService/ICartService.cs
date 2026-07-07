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
        Task<List<CartProductDTO>> GetBasketByUserId(int userId);
        Task AddProduct(int userId, int productId);
        Task IncreaseProductCount(int userId, int productId);
        Task DecrementProductCount(int userId, int productId);
        Task DeleteProductFromBasket(int userId, int productId);
    }
}
