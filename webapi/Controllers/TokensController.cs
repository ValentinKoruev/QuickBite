using QuickBite.Entities;
using QuickBite.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuickBite.Models.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] CreateModel model)
        {
            QuickBiteDbContext context = new QuickBiteDbContext();
            User u = context.Users
                .Where(i =>
                        i.Username == model.Username &&
                        i.Password == model.Password)
                .FirstOrDefault();

            if (u == null)
                return BadRequest(ModelState);

            var claims = new Claim[]
            {
                new Claim("LoggedUserId", u.Id.ToString())
            };

            var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("!Password123!Password123!Password123!Password123"));
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                "quickbite",
                "quickbite-api",
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signingCredentials
            );
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.WriteToken(token);

            foreach (var item in context.RefreshTokens.ToList())
              item.Status = RefreshToken.TokenStatus.Used;

            RefreshToken refreshToken = new RefreshToken();
            refreshToken.UserId = u.Id;
            refreshToken.Token = Guid.NewGuid().ToString();
            refreshToken.Status = RefreshToken.TokenStatus.Pending;

            context.RefreshTokens.Add(refreshToken);
            context.SaveChanges();

            return Ok(
                new
                {
                  success = true,
                  token = jwtToken,
                  refreshToken = refreshToken
                });
          }
    }
}
