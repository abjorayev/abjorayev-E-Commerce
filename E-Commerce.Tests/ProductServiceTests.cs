using AutoMapper;
using Castle.Core.Logging;
using E_Commerce.Application.DTO;
using E_Commerce.Application.ProductService;
using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IECommerceRepository<Product>> _productRepositoryMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IECommerceRepository<Product>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
                ); 
        }

        [Fact]
        public async Task Create_WhenValid_ShouldCreateProduct()
        {
            _mapperMock
    .Setup(x => x.Map<Product>(It.IsAny<ProductDTO>()))
    .Returns( new Product() { CategoryId = 1, DescriptionUz = "test", DescriptionRu = "test",
    ImageUrl = "test", Name = "test", Price = 10, ProductCount = 15} );

            var category = new Category { Id = 1 };
            var product = new ProductDTO
            {
                CategoryId = 1,
                DescriptionUz = "test",
                DescriptionRu = "test",
                ImageUrl = "test",
                Name = "test",
                Price = 10,
                ProductCount = 15
            };

            var action = async () => await _productService.Create(product);
            await action();
            _productRepositoryMock.Verify(x => x.Add(It.IsAny<Product>()), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenProductNotFound_ShouldReturnFalse()
        {
            var category = new Category() { Id = 1 };
            var product = new List<Product>() { new Product { Id = 1, CategoryId = 1} };

            var productMock = product.BuildMock<Product>();

            _productRepositoryMock
              .Setup(x => x.Query())
              .Returns(productMock);

            var action = await _productService.Delete(2);
            
            Assert.False(action);
        }

        [Fact]
        public async Task Delete_WhenProductExists_ShouldSetActiveToFalse()
        {
            var category = new Category() { Id = 1 };
            var product = new List<Product>() { new Product { Id = 1, CategoryId = 1, Active = true } };

            var productMock = product.BuildMock<Product>();

            _productRepositoryMock
              .Setup(x => x.Query())
              .Returns(productMock);

            var action = async () => await _productService.Delete(1);
            await action();

            _productRepositoryMock.Verify(
     x => x.Update(It.Is<Product>(p => p.Active == false)),
     Times.Once());
        }
    }
}
