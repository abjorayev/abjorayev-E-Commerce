using AutoMapper;
using E_Commerce.Application.DTO;
using E_Commerce.Domain.Entities;
using E_Commerce.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IECommerceRepository<Category> _categoryRepository;
        private ILogger<CategoryService> _logger;
        private IMapper _mapper;
        private IRedisService.IRedisService _redisService;

        public CategoryService(IECommerceRepository<Category> categoryRepository, ILogger<CategoryService> logger, IMapper mapper, IRedisService.IRedisService redisService)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _mapper = mapper;
            _redisService = redisService;   
        }

        public async Task<int> Create(CategoryDTO entity)
        {
            try
            {
                var category = _mapper.Map<Category>(entity);
                await _categoryRepository.Add(category);
                await _categoryRepository.SaveChanges();
                return category.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding category: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var category = await _categoryRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return false;

                await _categoryRepository.Delete(category);
                await _categoryRepository.SaveChanges();
                await _redisService.DeleteRedisData("category:{uz}");
                await _redisService.DeleteRedisData("category:{ru}");
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while deleting category: {ex.Message} {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<CategoryResponse>> GetAll(string lang)
        {
            var redis = await _redisService.GetDataAsync<List<CategoryResponse>>($"category:{lang}");
            if (redis != null)
                return redis;
            var result = await _categoryRepository.Query().Select(x => new CategoryResponse
            {
                Id = x.Id,
                Description = lang == "uz" ? x.DescriptionUz : x.DescriptionRu,
                Name = lang == "uz" ? x.NameUz : x.NameRu,
            }).ToListAsync();
            await _redisService.SetDataAsync<List<CategoryResponse>>($"category:{lang}", result, TimeSpan.FromDays(10));
            return result;
        }

        public async Task<bool> Update(CategoryDTO entity)
        {
            try
            {
                var category = await _categoryRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
                if (category == null) 
                    return false;
                category.DescriptionRu = entity.DescriptionRu;
                category.DescriptionUz = entity.DescriptionUz;
                category.NameUz = entity.NameUz;
                category.NameRu = entity.NameRu;
                await _categoryRepository.Update(category);
                await _categoryRepository.SaveChanges();

                await _redisService.DeleteRedisData("category:{uz}");
                await _redisService.DeleteRedisData("category:{ru}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating category: {ex.Message} {ex.StackTrace}");
                return false;
            }
        }
    }
}
