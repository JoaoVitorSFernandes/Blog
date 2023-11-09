using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]

public class AccountController : ControllerBase
{
    [HttpPost("v1/login")]
    public async Task<IActionResult> Login(
        [FromServices] TokenService tokenService)
    {
        try
        {
            var token = tokenService.GenerateToken(null);

            return Ok(new ResultViewModel<dynamic>(new { token }));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}