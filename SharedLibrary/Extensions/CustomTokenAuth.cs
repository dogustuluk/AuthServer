using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomTokenAuth
    {
        public static void AddCustomTokenAuth(this IServiceCollection services, CustomTokenOption tokenOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //bu kod satırı; üstteki kod satırı ile alttaki kod satırının birbiriyle iletişim
                //halinde olması sağlamaktadır.
                //3'ünün de ismi aynı olmalıdır ki birbirleriyle iletişim halinde olabilsinler.
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0], //audience'ların ilk indisini vermemizin sebebi bizim ana projemiz olmasıdır. ana projeyi vererek diğer
                    //audience'lara da erişim sağlatabiliriz.
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    //kontrol ediliyor
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //default olarak verilen 5dk'yı sıfırlar. farklı serverlarda çalışmaktan dolayı oluşan zamanlama hatalarını tolere edebilmek
                    //için default değer 5dk'dır. test amaçlı olması nedeniyle bunu 0'a çekeriz.
                    //.
                };
            });
        }
    }
}
