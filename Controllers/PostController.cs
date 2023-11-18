using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 25)
    {
        //Paginação de dados
        try
        {
            var count = await context.Posts.AsNoTracking().CountAsync();
            var posts = await context
                                .Posts
                                .AsNoTracking()
                                .Include(x => x.Category)
                                .Include(x => x.Author)
                                .Select(x => new ListPostViewModel
                                {
                                    Id = x.Id,
                                    Title = x.Title,
                                    Slug = x.Slug,
                                    LastUpdateDate = x.LastUpdateDate,
                                    Category = x.Category.Name,
                                    Author = $"{x.Author.Name} ({x.Author.Email})"
                                })
                                .Skip(page * pageSize)
                                .Take(pageSize)
                                .OrderByDescending(x => x.LastUpdateDate)
                                .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                page,
                pageSize,
                posts
            }));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<List<Post>>("06XE3 - Internal Server Error"));
        }
    }

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> GetAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        //Paginação de dados
        try
        {
            var post = await context
                                .Posts
                                .AsNoTracking()
                                .Include(x => x.Author)
                                .ThenInclude(x => x.Roles)
                                .Include(x => x.Category)
                                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
                return NotFound(new ResultViewModel<Post>("Post Not Found"));

            return Ok(new ResultViewModel<Post>(post));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResultViewModel<Post>($"06XE3 - Internal Server Error {ex.Message}"));
        }
    }


    [HttpGet("v1/posts/category/{category}")]
    public async Task<IActionResult> GetByCategoryAsync(
        [FromRoute] string category,
        [FromServices] BlogDataContext context,
        [FromQuery] int page = 0,
        [FromQuery] int sizePage = 25)
    {
        try
        {
            var count = await context
                                .Posts.AsNoTracking()
                                .Include(x => x.Category)
                                .Where(x => x.Category.Slug == category)
                                .CountAsync();
            
            var posts = await context
                                .Posts
                                .AsNoTracking()
                                .Include(x => x.Author)
                                .Include(x => x.Category)
                                .Where(x => x.Category.Slug == category)
                                .Select(x => new ListPostViewModel
                                {
                                    Id = x.Id,
                                    Title = x.Title,
                                    Slug = x.Slug,
                                    LastUpdateDate = x.LastUpdateDate,
                                    Category = x.Category.Name,
                                    Author = $"{x.Author.Name} ({x.Author.Email})"
                                })
                                .Skip(page * sizePage)
                                .Take(sizePage)
                                .OrderByDescending(x => x.LastUpdateDate)
                                .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                Total = count,
                page,
                sizePage,
                posts
            }));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<Post>("32XE5 - Internal Server Error"));
        }
    }
}