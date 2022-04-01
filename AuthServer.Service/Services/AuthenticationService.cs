using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entity;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
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
        private readonly IGenericRepository<UserRefreshToken> _userRefreshToken;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(UserManager<UserApp> userManager, IOptions<List<Client>> optionsClient, ITokenService tokenService,
            IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService  )
        {
            _userManager = userManager;
            _clients = optionsClient.Value;
            _userRefreshToken = userRefreshTokenService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }


        public Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            throw new NotImplementedException();
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
