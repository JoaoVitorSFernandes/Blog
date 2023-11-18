using Blog.Data;
using Blog.Extension;
using Blog.Models;
using Blog.ViewModels.Categories;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using sib_api_v3_sdk.Model;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] IMemoryCache cache,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return context.Categories.AsNoTracking().ToList();
                });

                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("02XE1 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context
                                .Categories
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("03XC1 - We were unable to find this category in our records"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("03XC2 - Internal erro in server of the application."));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<Category>(ModelState.GetErros()));

                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower(),
                    Posts = { }
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XC9 - Not was possible include the category"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XC9 - Not was possible include the category"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<Category>(ModelState.GetErros()));

                var category = await context
                                    .Categories
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("We were unable to find this category in our records"));

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (DbUpdateException)
            {                          //
                return StatusCode(500, new ResultViewModel<Category>("05XE8 - Not was possible update category."));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE8 - Not was possible update category."));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context
                                        .Categories
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("We were unable to find this category in our records"));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new { message = "Record deleted successfully" }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE7 - Not was possible delete category."));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("05X12 - Not was possible delete category."));
            }
        }


    }
}
