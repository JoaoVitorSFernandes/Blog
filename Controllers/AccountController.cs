using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Extension;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts/")]
    public async Task<IActionResult> PostAsync(
        [FromBody] RegisterViewModel model,
        [FromServices] EmailService emailService,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<User>(ModelState.GetErros()));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };

        //Recebe = length password - special caracter True or Flase - UperCase or no
        var password = PasswordGenerator.Generate(25); // 
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(
                    user.Name,
                    user.Email,
                    "Teste de envio de email",
                    $"Sua senha e {password}"
            );

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("05X99 - This email is already registered"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X99 - Internal Server Erro"));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] TokenService tokenService)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErros()));

            var user = await context
                        .Users
                        .AsNoTracking()
                        .Include(x => x.Roles)
                        .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Email or password invalid'."));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Email ou Password invalid."));

            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<string>("Internal Server Error"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Internal Server Error"));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context)
    {

        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"^data:imageV[a-z]+;base64,")
                .Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("06XE3 - Interal Server Erro"));
        }

        var user = await context
                            .Users
                            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
            return NotFound(new ResultViewModel<string>("User not found"));

        user.Image = $"https://localhost/images/{fileName}";

        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("06XE3 - Interal Server Erro"));
        }

        return Ok(new ResultViewModel<dynamic>(new {messgae= "Not i possible update one iamge"}));
    }
}