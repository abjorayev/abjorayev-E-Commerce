using E_Commerce.Application.CartService;
using E_Commerce.Application.RedisService;
using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using MockQueryable;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRedisService> _cartRedisServiceMock;
        private readonly Mock<IECommerceRepository<Product>> _productRepositoryMock;
        private readonly CartService _cartRedisService;

        public CartServiceTests()
        {
            _cartRedisServiceMock = new Mock<ICartRedisService>();
            _productRepositoryMock = new Mock<IECommerceRepository<Product>>();
            _cartRedisService = new CartService(
                _cartRedisServiceMock.Object,
                _productRepositoryMock.Object
                );
        }
        [Fact]
        public async Task AddProduct_WhenProductNotFound_ShouldThrowException()
        {
            var products = new List<Product>()
            {
                new Product { Id = 1, }
            };

            var productsMock = products.BuildMock<Product>();
            _productRepositoryMock.Setup(x => x.Query()).Returns(productsMock);

            var action = async () => await _cartRedisService.AddProduct("TestUserId", 2);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }

        [Fact]
        public async Task AddProduct_WhenValid_ShouldAddToCart()
        {
            var products = new List<Product>()
            {
                new Product { Id = 1,ProductCount = 15 }
            };

            var productsMock = products.BuildMock<Product>();
            _productRepositoryMock.Setup(x => x.Query()).Returns(productsMock);

            var action = async () => await _cartRedisService.AddProduct("TestUserId", 1);

            await action();

           _cartRedisServiceMock.Verify(x => x.AddOrUpdateItem("TestUserId", 1, 1), Times.Once());
        }

        [Fact]
        public async Task DeleteBasket_ShouldClearCart()
        {
            var action = async () => await _cartRedisService.DeleteBasket("TestUserId");

            await action();
            _cartRedisServiceMock.Verify(x => x.Delete("TestUserId"), Times.Once());
        }
    }
}
