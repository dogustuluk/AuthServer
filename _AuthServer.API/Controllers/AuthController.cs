﻿using AuthServer.Core.DTOs;
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
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var result = await _authenticationService.CreateTokenAsync(loginDto);

            //if (result.StatusCode == 200)
            //{
            //    return Ok(result);
            //}
            //else if (result.StatusCode == 404)
            //{
            //    return NotFound(result);
            //}
            //gibi bir işleme gerek kalmadı CustomBaseController sayesinde. alttaki kod direkt olarak diğer controller'dan aldığı statuscode'u bize vermektedir.
            //CustomBaseController'ı implemente etmeyi unutma

            return ActionResultInstance(result);
        }

    }
}
