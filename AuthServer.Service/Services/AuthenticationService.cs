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

            var user = await _userManager.FindByEmailAsync(loginDto.Email); //kullanıcının email ini buluyoruz
            
            if (user == null) return Response<TokenDto>.Fail("Email or password is wrong", 400, true); //email veya şifre hatalı mesajının olmasının
            //sebebi ise kötü niyetli kullanıcının çıkarım yapmasının önüne geçmektir.

            if(!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or password is wrong", 400, true);
            }
            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshTokenService.where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, 
                    Expiration = token.RefreshTokenExpiration });

            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200);

            
        }

        public Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            throw new NotImplementedException();
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
