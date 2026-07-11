using AutoMapper;
using E_Commerce.Application.DTO;
using E_Commerce.Application.Response;
using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce.Application.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IECommerceRepository<Product> _productRepository;
        private ILogger<ProductService> _logger;
        private IMapper _mapper;

        public ProductService(IECommerceRepository<Product> productRepository, ILogger<ProductService> logger, IMapper mapper)
        {
            _productRepository = productRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Create(ProductDTO entity)
        {
            try
            {
                var product = _mapper.Map<Product>(entity);
                product.CreatedAt = DateTime.UtcNow;
                product.Active = true;
                await _productRepository.Add(product);
                await _productRepository.SaveChanges();
                return product.Id;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while adding product: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var product = _productRepository.Query().FirstOrDefault(x => x.Id == id && x.Active);
                if(product == null)
                {
                    _logger.LogInformation($"Product with {id} is does not exist");
                    return false;
                }
                product.Active = false;
                await _productRepository.Update(product);
                await _productRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting product: {ex.Message} {ex.StackTrace}");
                throw ;
            }
        }

        public async Task<PaginatedResponse<ProductResponse>> GetAll(
     int? minPrice,
     int? maxPrice,
     string? search,
     string lang,
     int page = 1,
     int rows = 20)
        {
            var products = _productRepository.Query()
                .Where(x => x.Active);

            if (minPrice > 0)
                products = products.Where(x => x.Price >= minPrice);

            if (maxPrice > 0)
                products = products.Where(x => x.Price <= maxPrice);

            if (!string.IsNullOrWhiteSpace(search))
                products = products.Where(x => x.Name.Replace(" ", "").ToLower().Contains(search.Replace(" ", "").ToLower()));

            var totalCount = await products.CountAsync();

            var pagedProducts = await products
                .Skip((page - 1) * rows)
                .Take(rows)
                .ToListAsync();

            var result = pagedProducts.Select(x => new ProductResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = lang == "uz" ? x.DescriptionUz : x.DescriptionRu,
                CategoryId = x.CategoryId,
                ImageUrl = x.ImageUrl,
                Price = x.Price,
                ProductCount = x.ProductCount
            }).ToList();

            return new PaginatedResponse<ProductResponse>
            {
                Items = result,
                TotalCount = totalCount,
                Page = page,
                PageSize = rows
            };
        }

        public async Task<ProductResponse> GetById(int id, string lang)
        {
            var product = await _productRepository.Query().FirstOrDefaultAsync(x => x.Id == id && x.Active);
            if(product == null)
            {
                _logger.LogInformation($"Product with {id} is does not exist");
                return null; 
            }
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = lang == "uz" ? product.DescriptionUz : product.DescriptionRu,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                ProductCount = product.ProductCount
            };
        }

        public async Task<bool> Update(ProductDTO entity)
        {
            try
            {
                var product = await _productRepository.Query().FirstOrDefaultAsync(x => x.Id == entity.Id);
                if (product == null)
                    return false;
                product.DescriptionRu = entity.DescriptionRu;
                product.CategoryId = entity.CategoryId;
                product.ImageUrl = entity.ImageUrl;
                product.Price = entity.Price;
                product.ProductCount = entity.ProductCount;
                product.DescriptionUz = entity.DescriptionUz;
                product.Name = entity.Name;
               
                await _productRepository.Update(product);
                await _productRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating Product: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

       
    }
}
