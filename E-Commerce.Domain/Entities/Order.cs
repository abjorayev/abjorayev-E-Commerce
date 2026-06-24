using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }

    public enum OrderStatus
    {
        InProcess = 0,
        OnWay = 1,
        Delivered = 2
    }
}
