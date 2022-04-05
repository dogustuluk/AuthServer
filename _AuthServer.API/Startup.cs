using AuthServer.Core.Configuration;
using AuthServer.Core.Entity;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _AuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //DI Register kodlarý
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic olan yapýlarda type'ýný belli ederiz.
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>)); //generic içerisinde iki adet entity döndüðü için "<>"
                                                                                       //içerisinde kaç adet entity varsa virgül konur.
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //yukarýdaki kodlarla constructorlarda geçmiþ olduðumuz class'larý DI Container'a ekledik.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"),sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("authserver.data"); //Projenin ismi ne ise Assembly'nin ismi de o olmalýdýr.
                }); //veri tabaný yolunu bildirmiþ oluyoruz.
            });


            //DI Container'a ekliyoruz
            //Kullanýcýnýn "UserApp" olduðunu belirtiyoruz, rolü ise identity kütüphanesinden gelen default role. ek olarak rol eklemek istersek UserApp classýnda
            //ilgili alanda açýklama satýrý halinde bir örnek mevcuttur.
            services.AddIdentity<UserApp, IdentityRole>(Opt =>
            {
                Opt.User.RequireUniqueEmail = true; //Email'in veri tabanýnda benzersiz olmasý için gerekli kod.
                Opt.Password.RequireNonAlphanumeric = false; // '*,?,-' gibi karakterlerin þifre alanýnda zorunlu olmasýný engelleyen kod.
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //"AddDefaultTokenProviders" >> þifre sýfýrlama gibi iþlemlerde yeni bir token 
            //üretmek için gerekiyor.


            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //bu kod satýrý; üstteki kod satýrý ile alttaki kod satýrýnýn birbiriyle iletiþim
                //halinde olmasý saðlamaktadýr.
                //3'ünün de ismi ayný olmalýdýr ki birbirleriyle iletiþim halinde olabilsinler.
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0], //audience'larýn ilk indisini vermemizin sebebi bizim ana projemiz olmasýdýr. ana projeyi vererek diðer
                    //audience'lara da eriþim saðlatabiliriz.
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    //kontrol ediliyor
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //default olarak verilen 5dk'yý sýfýrlar. farklý serverlarda çalýþmaktan dolayý oluþan zamanlama hatalarýný tolere edebilmek
                    //için default deðer 5dk'dýr. test amaçlý olmasý nedeniyle bunu 0'a çekeriz.
                    //.
                };
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "_AuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "_AuthServer.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
