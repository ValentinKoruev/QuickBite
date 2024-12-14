using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuickBite.Entities;
using QuickBite.Models.RefreshTokens;
using QuickBite.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class RefreshTokensController : ControllerBase
  {
      [HttpPost]
      public IActionResult Post([FromBody] CreateModel model)
      {
          QuickBiteDbContext context = new QuickBiteDbContext();
          RefreshToken t = context.RefreshTokens
              .Where(i =>
                      i.Token == model.RefreshToken)
              .FirstOrDefault();

          if (t == null)
            return BadRequest(ModelState);

          User u = context.Users
            .Where(u => u.Id == t.UserId)
            .FirstOrDefault();

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

          foreach(var item in context.RefreshTokens.ToList())
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
