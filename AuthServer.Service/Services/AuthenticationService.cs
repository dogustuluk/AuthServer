using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entity;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly List<Client> _clients;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(UserManager<UserApp> userManager, IOptions<List<Client>> optionsClient, ITokenService tokenService,
            IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService  )
        {
            _userManager = userManager;
            _clients = optionsClient.Value;
            _userRefreshTokenService = userRefreshTokenService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }


        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            //if'lerle tek tek kontrol etmeye "savunmacı yaklaşım" denmektedir. iç içe if-else blokları yazılmaz
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto)); //eğer loginDto boş ise hata fırlat
            //nameoff >> kimin hakkında olduğunu belirtir. 

            var user = await _userManager.FindByEmailAsync(loginDto.Email); //email adresinden user'ı buluyoruz.
            
            if (user == null) return Response<TokenDto>.Fail("Email or password is wrong", 400, true); //email veya şifre hatalı mesajının olmasının
            //sebebi ise kötü niyetli kullanıcının çıkarım yapmasının önüne geçmektir.

            if(!await _userManager.CheckPasswordAsync(user, loginDto.Password)) //şifre kontrolünü yapıyoruz. password hashlendiği için burada password'ü vermek gerekiyor.
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 400, true);
            }
            var token = _tokenService.CreateToken(user); //bu aşamada kullanıcı var olduğu bilindiği için kullanıcıya bir token veriyoruz.

            //ardından refreshtoken kontrolü yapmamız lazım fakat öncesinde veri tabanında bir refresh token olup olmadığını kontrol etmeliyiz.

            var userRefreshToken = await _userRefreshTokenService.where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, 
                    Expiration = token.RefreshTokenExpiration }); //veri tabanına kayıt işlemi gerçekleşmesini sağlayan kod.

            }
            else //güncelleme işlemi gerçekleştirilir.
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }
            //buraya kadar henüz veri tabanına işlem yansıtılmadı.
            //entity framework işlemi memory'de tutuyor şuanda.
            await _unitOfWork.CommitAsync(); //işlemin veri tabanına kayıt edilmesini sağlayan kod.
            return Response<TokenDto>.Success(token, 200); //token'ın başarılı olarak kaydedildiğini "200" durum koduyla dönen kod.

            
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.ClientId == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);

            if (client == null) //üstteki kod client'ı bulamadıysa (yani öyle bir client yok ise) bu blokta fail veriyoruz
            {
                return Response<ClientTokenDto>.Fail("Clientid or Secretid not found", 404, true); //404 hata kodu "not found" ifadesinde kullanılır.
            }

            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }

        public Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
