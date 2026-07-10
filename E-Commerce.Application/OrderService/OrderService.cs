using AutoMapper;
using E_Commerce.Application.DTO;
using E_Commerce.Application.Response;
using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Commerce.Application.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IECommerceRepository<Order> _orderRepository;
        private readonly IECommerceRepository<OrderItem> _orderItemRepository;
        private readonly IECommerceRepository<Product> _productRepository;
        private ILogger<OrderService> _logger;
        private IMapper _mapper;


        public OrderService(IECommerceRepository<Order> orderRepository, IECommerceRepository<OrderItem> orderItemRepository, 
            ILogger<OrderService> logger, IMapper mapper, IECommerceRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
        }
        //TotalAmount -> idk how to for now :)
        public async Task<int> Create(OrderCreateDTO entity)
        {
            try
            {
                var products = _productRepository.Query().Where(x => entity.Items.Select(x => x.ProductId).Contains(x.Id)).ToList();
                foreach(var item in entity.Items)
                {
                    var product = products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product == null || product.ProductCount < item.ProductCount)
                        throw new Exception($"Product {item.ProductId} is out of stock");
                }
                var order = new Order
                {
                    UserId = entity.UserId,
                    DeliveryAddress = entity.DeliveryAddress,
                    CreatedAt = DateTime.UtcNow,
                    OrderStatus = OrderStatus.InProcess,
                    OrderItems = entity.Items.Select(x =>
                    {
                        var productInfo = products.FirstOrDefault(p => p.Id == x.ProductId);
                        return new OrderItem
                        {
                            ProductId = x.ProductId,
                            ProductCount = x.ProductCount,
                            PriceAtPurchase = productInfo?.Price * x.ProductCount ?? 0
                        };
                    }),
                    TotalAmount = entity.TotalAmount,
                };
                //var order = _mapper.Map<Order>(entity);
                order.CreatedAt = DateTime.UtcNow;
                await _orderRepository.Add(order);

                foreach(var item in entity.Items)
                {
                    var product = products.FirstOrDefault(x => x.Id == item.ProductId);
                    product.ProductCount -= item.ProductCount;
                    await _productRepository.Update(product);
                }

                await _orderRepository.SaveChanges();
                return order.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding order: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var orderItems = _orderItemRepository.Query().Where(x => x.OrderId == id).ToList();
                if(orderItems.Count > 0)
                   await _orderItemRepository.DeleteRange(orderItems);

                var order = await _orderRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
                if (order == null)
                    return false;

                await _orderRepository.Delete(order);
                await _orderRepository.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while deleting order: {ex.Message} {ex.StackTrace}");
                return false;
            }
        }
       
        public async Task<OrderByIdResponse> GetById(int id)
        {
            var order = await  _orderRepository.Query().Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return null;
            var products = await _productRepository.Query().Where(x => order.OrderItems.Select(x => x.ProductId).Contains(x.Id)).ToListAsync();
            return new OrderByIdResponse
            {
                Id = order.Id,
                OrderStatus = order.OrderStatus,
                CreatedAt = order.CreatedAt,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                Items = order?.OrderItems?.Select(x => 
                {
                    var product = products.FirstOrDefault(p => p.Id == x.ProductId);
                    return new OrderItemDTO
                    {
                        ProductId = x.ProductId,
                        ProductName = product?.Name ?? "",
                        ProductPhoto = product?.ImageUrl ?? "",
                        ProductCount = x.ProductCount,
                        PriceAtPurchase = product?.Price * x.ProductCount ?? 0
                    };

                }).ToList() ?? new List<OrderItemDTO>(),
            };
        }

        public async Task<PaginatedResponse<OrderDTO>> GetByUserId(string userId, int page = 1, int rows = 20)
        {
            var orders = _orderRepository.Query().Where(x => x.UserId == userId);
            var totalCount = await orders.CountAsync();
            var pagedOrders =  orders
                .Skip((page - 1) * rows)
                .Take(rows)
                .ToList();

            var mapper = _mapper.Map<List<OrderDTO>>(pagedOrders);

            return new PaginatedResponse<OrderDTO>
            {
                Items = mapper.ToList(),
                Page = page,
                PageSize = rows,
                TotalCount = totalCount
            };
        }

        public async Task UpdateOrderStatus(OrderStatus status, int orderId)
        {
            var order = await _orderRepository.Query().FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null)
                return;

            order.OrderStatus = status;
            await _orderRepository.Update(order);
            await _orderRepository.SaveChanges();
        }
    }
}
