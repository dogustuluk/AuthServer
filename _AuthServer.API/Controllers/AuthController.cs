using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")] //metot ismi ile erişim yapabilmek için [action] ekledik. Temel crud işlemleri olsaydı yapmamız gerekmezdi.
    //erişim örneği >> api/auth/createtoken şeklinde olacak. birden fazla metodumuz olduğu için böyle yapıyoruz.
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


    }
}
