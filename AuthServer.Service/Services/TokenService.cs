using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using Microsoft.Extensions.Options;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

        //Token'ın Payload'ında olmasını istediğimiz dataları eklemek için aşağıdaki kodlar yazılır
        //Token'ın payloadında tutulan key-value'ların hepsi claim nesnesinden gelir.
        //audiences >> bu token'ın hangi api'lere istek yapacağına karşılık gelmektedir.
        public IEnumerable<Claim> GetClaim(UserApp userApp, List<String> audiences)
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userApp.Id), //token eğer üyelik ile ilgiliyse Id olmak zorunda
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email), //new Claim("email", userApp.Email) şeklinde de yazılır ama her zaman 
                // const'lar üzerinden çalışmak daha sürdürülebilir ve yönetilebilir bir yöntem olmaktadır. sabitler yanlış kod yazmanın da önüne geçer.
                new Claim(ClaimTypes.Name, userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //token'a id vermek zorunlu değildir, ama daha doğru bir yapı inşa etmek
                //için tokenlara id vermemiz gerekmektedir.
            };

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
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
