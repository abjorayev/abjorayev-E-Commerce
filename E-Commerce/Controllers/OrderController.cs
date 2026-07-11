using E_Commerce.Application.DTO;
using E_Commerce.Application.OrderService;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            order.UserId = userId;
            var result = await _orderService.Create(order);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Delivery")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            return Ok(_orderService.UpdateOrderStatus(orderStatus, orderId));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders(int page = 1, int rows = 20)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            return Ok(await _orderService.GetByUserId(userId, page, rows));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null) return Unauthorized();
            return Ok(await _orderService.GetById(id, userId));
        }
    }
}
