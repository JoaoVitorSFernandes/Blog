using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Blog.Extension;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
        => _configuration = configuration;

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("ApiKeyConfigurations:JwtKey"));
        var claims = user.GetClaims();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}