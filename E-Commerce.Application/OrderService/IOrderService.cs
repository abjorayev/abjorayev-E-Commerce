using E_Commerce.Application.ApplicationService;
using E_Commerce.Application.DTO;
using E_Commerce.Application.Response;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.OrderService
{
    public interface IOrderService 
    {
        Task<PaginatedResponse<OrderDTO>> GetByUserId(string userId, int page = 1, int rows = 20);
        Task<OrderByIdResponse> GetById(int id, string userId);
        Task UpdateOrderStatus(OrderStatus status, int orderId);
        Task<bool> Delete(int id);
        Task<int> Create(OrderCreateDTO createDTO);
    }
}
