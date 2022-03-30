using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using Microsoft.Extensions.Options;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserApp _userApp;
        private readonly CustomTokenOption _tokenOption;

        public TokenService(UserApp userApp, IOptions<CustomTokenOption>options)
        {
            _userApp = userApp;
            _tokenOption = options.Value;
        }

        private string CreateRefreshToken()
        {
            // return Guid.NewGuid().ToString(); //bu yol da kullanılabilir ama oldukça unique olması için bunu kullanmıyor olucaz.
            var numberByte = new Byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
            //bu şekilde oluşturulan kriptografinin imzası benzersiz olacaktır, birbirinin aynısı olma ihtimali çok düşüktür.
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            throw new NotImplementedException();
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
