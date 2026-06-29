using E_Commerce.Application.ApplicationService;
using E_Commerce.Application.DTO;
using E_Commerce.Application.Response;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.ProductService
{
    public interface IProductService : IApplicationService<ProductDTO>
    {
        Task<PaginatedResponse<ProductDTO>> GetAll(int minPrice, int maxPrice, string search, int page = 1, int rows = 20);
       Task< ProductDTO> GetById(int id);
    }
}
