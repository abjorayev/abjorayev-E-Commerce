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
                product.CreatedAt = DateTime.Now;
                await _productRepository.Add(product);
                return product.Id;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while adding product: {ex.Message} {ex.StackTrace}");
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var product = _productRepository.Query().FirstOrDefault(x => x.Id == id && x.Active);
                if(product == null)
                {
                    _logger.LogInformation($"Product with {id} is does not exist");
                    return;
                }
                await _productRepository.Delete(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting product: {ex.Message} {ex.StackTrace}");
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResponse<ProductDTO>> GetAll(
     int minPrice,
     int maxPrice,
     string search,
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
                products = products.Where(x => x.Name.ToLower().Contains(search.ToLower()));

            var totalCount = await products.CountAsync();

            var pagedProducts = await products
                .Skip((page - 1) * rows)
                .Take(rows)
                .ToListAsync();

            var result = _mapper.Map<List<ProductDTO>>(pagedProducts);

            return new PaginatedResponse<ProductDTO>
            {
                Items = result,
                TotalCount = totalCount,
                Page = page,
                PageSize = rows
            };
        }

        public async Task<ProductDTO> GetById(int id)
        {
            var product = _productRepository.Query().FirstOrDefault(x => x.Id == id);
            if(product == null)
            {
                _logger.LogInformation($"Product with {id} is does not exist");
                return null; 
            }
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task Update(ProductDTO entity)
        {
            try
            {
                var product = _mapper.Map<Product>(entity);
                await _productRepository.Update(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating Product: {ex.Message} {ex.StackTrace}");
                throw new Exception(ex.Message);
            }
        }

       
    }
}
