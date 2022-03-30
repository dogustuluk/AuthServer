using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {   //data'yı alana kadar memory'de bulunmaması için lazy kullanırız.
        //Lazy class'ı ile ihtiyaç halinde yüklenmesini istediğimiz data ları çekeriz, gereksiz bir şekilde tüm datalar yüklenmez.
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg => //action metotlar parametre alır ama geriye herhangi bir değer döndürmez.
            {
                cfg.AddProfile<DtoMapper>();
                
            });
            return config.CreateMapper();
        });

        public static IMapper Mapper = lazy.Value;
    }
}
