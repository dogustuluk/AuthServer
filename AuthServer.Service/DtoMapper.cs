using AuthServer.Core.DTOs;
using AuthServer.Core.Entity;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    class DtoMapper: Profile
    {
        public DtoMapper()
        {
            CreateMap<ProductDto, Product>().ReverseMap(); //reversemap ile işlemin tersinin de olabileceği potansiyeliyle yapıyoruz.
            CreateMap<UserAppDto, UserApp>().ReverseMap();
        }
    }
}
