using AuthServer.Core.DTOs;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {//yeni bir kullanıcı kaydı için:
            var user = new UserApp {Email = createUserDto.Email, UserName = createUserDto.UserName };//password hashleme işlemini burda yapmıyoruz

            var result = await _userManager.CreateAsync(user, createUserDto.Password);//createUserDto.Password ile yukarıda yapmadığımız
                                                                                      //hashleme işlemini burda yapmış bulunmaktayız.
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();//birden fazla hata olabilme durumu olabilir.
                return Response<UserAppDto>.Fail(new ErrorDto(errors,true),400);//client hatası olduğu için durum kodu "400" olarak atandı
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);//userAppDto dönmemiz gerekiyor fakat elimizde
                                                                                                //user var. bundan dolayı mapleme işlemi yapıyoruz.
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Response<UserAppDto>.Fail("Username is not found", 404, true);
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);

        }
    }
}
