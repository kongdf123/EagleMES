using EagleMES.Api.Data;
using EagleMES.Api.DTOs;
using EagleMES.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EagleMES.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            var user = await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                usename = user.Username,
                role = user.Role
            });
        }

        private object GenerateJwtToken(SysUser user)
        {
            var claims = new[] { 
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
