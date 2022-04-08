using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
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
    public class TokenService : ITokenService //normalde bu servis dış dünyaya açılmaz, DI Container yardımıyla istenilen class'a initialize edilir.
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
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
        public IEnumerable<Claim> GetClaims(UserApp userApp, List<String> audiences)
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

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            //burada üyelik istemeyen kısımlardaki tokenları yazıyoruz.
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString()); //bu kod parçası ile token'ın kime ait olduğu bulunur.
            return claims;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration); //token'ın erişim süresi
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration); //token yenilenme süresi
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey); //token imzası
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            //yukarıdaki kod satırında imza oluşturulur. algoritmadan istenilen herhangi bir şifreleme algoritması seçilebilir.

            //alt satırdaki kod bloğu ile token'ımızı oluşturmaya başlıyoruz
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                issuer: _tokenOption.Issuer, //Token'ı yayınlayanın kim olduğunu buluyoruz. Issuer ise "_tokenOption"dan gelmektedir. 
                //_tokenOption ise "CustomTokenOption"dan gelir.
                expires: accessTokenExpiration, //token'ın ömrünü veriyoruz.
                notBefore: DateTime.Now, //geçerli olan token süresinden önce geçersiz olmaması için.
                claims: GetClaims(userApp, _tokenOption.Audience), //token'ın hangi api'lere erişebileceği bilgisini vermiş oluyoruz.
                signingCredentials: signingCredentials //imzayı veriyoruz.
                //henüz token oluşmadı
                );

            //alttaki kod token'ı oluşturacak 
            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken); //burdan gelecek örnek data için jwt.io'daki encoded kısmına bakabilirsin

            //gelen token'ı tokenDto'ya dönüştürmemiz gerekmektedir.
            var tokenDto = new TokenDto
            {
                AccessToken = token, //"token" ile "accessToken" aynı şeydir.
                RefreshToken = CreateRefreshToken(), //string döndüğü için convert işlemi yapmıyoruz.
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            }; //dto artık hazır
            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
                );
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var clientTokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration
            };
            return clientTokenDto;

        }
    }
}
