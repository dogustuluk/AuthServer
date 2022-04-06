using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        [Authorize] //authorize olarak işaretlememizin sebebi; bu kod bloğunun mutlaka bir token istemesi kaynaklıdır.
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            return ActionResultInstance(await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name)); //claim içerisinde username'i name ile aldığımız için
            //ilgili token'ın payload'ından alanı alıyor (TokenService'te GetClaim method)
            //eğer düzgün bir isimlendirme ile yapılmazsa claim'lerde dolaşıp alakalı olan ismi aratıyor olacaktık. örnek aşağıdaki kod satırı.
            //HttpContext.User.Claims.Where(x => x.Type == "username").FirstorDefault(); /////
        }
    }
}
