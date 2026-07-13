using E_Commerce.Application.CartService;
using E_Commerce.Application.RedisService;
using E_Commerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            var result = await _cartService.GetBasketByUserId(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToBasket(int productId)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            await _cartService.AddProduct(userId, productId);
            return Ok();
        }
        [HttpPut("increase")]
        public async Task<IActionResult> IncreaseProductCount(int productId)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            await _cartService.IncreaseProductCount(userId, productId);
            return Ok();
        }

        [HttpPut("decrement")]
        public async Task<IActionResult> DecrementProductCount(int productId)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            await _cartService.DecrementProductCount(userId, productId);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductFromBasket(int productId)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            await _cartService.DeleteProductFromBasket(userId, productId);
            return Ok();
        }

        [HttpDelete("DeleteBasket")]
        public async Task<IActionResult> DeleteBasket()
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            await _cartService.DeleteBasket(userId);
            return Ok();
        }
    }
}
