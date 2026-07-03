using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTO
{
    public class OrderItemDTO
    {
      //  public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductPhoto { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int PriceAtPurchase { get; set; }
    }
    public class OrderCreateItemDTO
    {
        public int ProductId { get; set; }
        public int ProductCount { get; set; }
    }

}
