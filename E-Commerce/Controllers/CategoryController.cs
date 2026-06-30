using E_Commerce.Application.CategoryService;
using E_Commerce.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO category)
        {
            var result = await _categoryService.Create(category);
            if (result == 0)
                return BadRequest("Something get wrong");

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] CategoryDTO category)
        {
            var result = await _categoryService.Update(category);
            if (!result)
                return BadRequest("Something get wrong");

            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.Delete(id);
            if (!result)
                return BadRequest("Something get wrong");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string lang = Request.Headers["Accept-Language"].FirstOrDefault() ?? "ru";
            return Ok(await _categoryService.GetAll(lang));
        }
    }
}
