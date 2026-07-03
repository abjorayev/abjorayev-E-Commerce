using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class OrderCreateDTO
    {
        public string UserId { get; set; }
        public string DeliveryAddress { get; set; }
        public int TotalAmount { get; set; }
        public List<OrderCreateItemDTO> Items { get; set; } = new();
        
    }

    public class OrderByIdResponse
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }
}
