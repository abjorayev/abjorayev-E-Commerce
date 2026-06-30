using E_Commerce.Application.DTO;
using E_Commerce.Application.ProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
           _productService = productService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDTO product)
        {
            var result = await _productService.Create(product);
            if (result == 0)
                return BadRequest("Something get wrong");
            if (product.CategoryId == 0)
                return BadRequest("Category can't be null");

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductDTO product)
        {
            var result = await _productService.Update(product);
            if (!result)
                return BadRequest("Something get wrong");

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.Delete(id);
            if (!result)
                return BadRequest("Something get wrong");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _productService.GetById(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int minPrice, int maxPrice, string search, int page = 1, int row = 20)
        {
            return Ok(await _productService.GetAll(minPrice, maxPrice, search, page, row));
        }
    }
}
