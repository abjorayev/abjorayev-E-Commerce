using E_Commerce.Application.ApplicationService;
using E_Commerce.Application.DTO;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.CategoryService
{
    public interface ICategoryService : IApplicationService<CategoryDTO>
    {
        Task<List<CategoryResponse>> GetAll(string lang);
    }
}
