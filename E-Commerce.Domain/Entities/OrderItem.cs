using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductCount { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int PriceAtPurchase { get; set; }
    }
}
