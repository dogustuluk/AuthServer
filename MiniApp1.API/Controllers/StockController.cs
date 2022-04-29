using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniApp1.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name; //endpoint'e istek yapıldığında token'ın payload'ından name verisini çeker.
            //yukarıdaki name ile veri tabanından istenilen kullanıcıya ait verileri çekebiliriz.
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            //veri tabanından userName veya userId alanları üzerinden gerekli dataları çekebiliriz.
            var userMail = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email); //token'dan mail adresini almak için.
            //bu verileri çekerken ezbere gitmemek için TokenService'teki ilgili metot içerisinden (genellikle GetClaims olmakta) alınır.
            return Ok($"Stock Transactions => UserName: {userName} - UserId: {userIdClaim.Value} - UserMail: {userMail}");

        }

    }
}
