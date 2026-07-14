using AutoMapper;
using E_Commerce.Application.CartService;
using E_Commerce.Application.CategoryService;
using E_Commerce.Application.DTO;
using E_Commerce.Application.OrderService;
using E_Commerce.Application.ProductService;
using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly Mock<IECommerceRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IECommerceRepository<OrderItem>> _orderItemRepositoryMock;
        private readonly Mock<IECommerceRepository<Product>> _productRepositoryMock;
        private readonly Mock<IECommerceRepository<Category>> _categoryRepositoryMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly Mock<ILogger<ProductService>> _productLogger;
        private readonly Mock<ILogger<CategoryService>> _categoryLogger;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public OrderServiceTests()
        {

            _cartServiceMock = new Mock<ICartService>();
            _orderRepositoryMock = new Mock<IECommerceRepository<Order>>();
            _orderItemRepositoryMock = new Mock<IECommerceRepository<OrderItem>>();
            _productRepositoryMock = new Mock<IECommerceRepository<Product>>();
            _categoryRepositoryMock = new Mock<IECommerceRepository<Category>>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _mapperMock = new Mock<IMapper>();
            _productLogger = new Mock<ILogger<ProductService>>();
            _categoryLogger = new Mock<ILogger<CategoryService>>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _orderItemRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _productRepositoryMock.Object,
                _cartServiceMock.Object
            );

            _categoryService = new CategoryService(
                _categoryRepositoryMock.Object,
                _categoryLogger.Object,
                _mapperMock.Object
            );

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _productLogger.Object,
                _mapperMock.Object
                );
        }

        [Fact]
        public async Task Create_WhenCartIsEmpty_ShouldThrowException()
        {
            // Arrange
            _cartServiceMock
                .Setup(x => x.GetBasketByUserId(It.IsAny<string>()))
                .ReturnsAsync(new List<CartProductDTO>());

            var dto = new OrderCreateDTO { DeliveryAddress = "Test", UserId = "TestUserID" };

            // Act
            var action = async () => await _orderService.Create(dto);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }

        [Fact]
        public async Task Create_WhenProductOutOfStock_ShouldThrowException()
        {
            var cartItems = new List<CartProductDTO>
            {
                new CartProductDTO { ProductId = 1, ProductQuantity = 10 }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, ProductCount = 5 }
            }.AsQueryable();

            _cartServiceMock
                .Setup(x => x.GetBasketByUserId(It.IsAny<string>()))
                .ReturnsAsync(cartItems);

            _productRepositoryMock
                .Setup(x => x.Query())
                .Returns(products);

            var dto = new OrderCreateDTO { DeliveryAddress = "Test", UserId = "TestUserID" };

            // Act
            var action = async () => await _orderService.Create(dto);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }

        [Fact]
        public async Task Create_WhenValid_ShouldClearCartAfterOrder()
        {
            var cartItems = new List<CartProductDTO>
            {
                new CartProductDTO { ProductId = 1, ProductQuantity = 10 }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, ProductCount = 50 }
            }.AsQueryable();

            _cartServiceMock
                .Setup(x => x.GetBasketByUserId(It.IsAny<string>()))
                .ReturnsAsync(cartItems);

            _productRepositoryMock
                .Setup(x => x.Query())
                .Returns(products);

            _orderRepositoryMock
        .Setup(x => x.Add(It.IsAny<Order>()))
        .Returns(Task.CompletedTask);

            _orderRepositoryMock
                .Setup(x => x.SaveChanges())
                .Returns(Task.CompletedTask);

            _cartServiceMock
                .Setup(x => x.DeleteBasket(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var dto = new OrderCreateDTO { DeliveryAddress = "Test", UserId = "TestUserID" };

            // Act
            var action = async () => await _orderService.Create(dto);
            await action();
            _cartServiceMock.Verify(x => x.DeleteBasket("TestUserID"), Times.Once);
        }
        [Fact]
        public async Task Create_WhenValid_ShouldDecreaseProductStock()
        {
            var cartItems = new List<CartProductDTO>
            {
                new CartProductDTO { ProductId = 1, ProductQuantity = 10 }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, ProductCount = 50 }
            }.AsQueryable();

            _cartServiceMock
                .Setup(x => x.GetBasketByUserId(It.IsAny<string>()))
                .ReturnsAsync(cartItems);

            _productRepositoryMock
                .Setup(x => x.Query())
                .Returns(products);

            _orderRepositoryMock
        .Setup(x => x.Add(It.IsAny<Order>()))
        .Returns(Task.CompletedTask);

            _orderRepositoryMock
                .Setup(x => x.SaveChanges())
                .Returns(Task.CompletedTask);

            _cartServiceMock
                .Setup(x => x.DeleteBasket(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var dto = new OrderCreateDTO { DeliveryAddress = "Test", UserId = "TestUserID" };

            // Act
            var action = async () => await _orderService.Create(dto);
            await action();
            _productRepositoryMock.Verify(
     x => x.Update(It.Is<Product>(p => p.ProductCount == 40)),
     Times.Once());
        }

        [Fact]
        public async Task Delete_WhenOrderNotFound_ShouldReturnFalse()
        {
            var orders = new List<Order>()
            {
                new Order{ Id = 1, }
            }.AsQueryable();

            var orderItems = new List<OrderItem>()
            {
                new OrderItem {Id = 1, OrderId = 1}
            }.AsQueryable();

            _orderRepositoryMock
               .Setup(x => x.Query())
               .Returns(orders);

            _orderItemRepositoryMock
                .Setup(x => x.Query())
                .Returns(orderItems);

           
            var result = await _orderService.Delete(2);
            
            Assert.False(result);
        }
    }
}
