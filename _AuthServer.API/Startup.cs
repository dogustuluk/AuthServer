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
            //DI Register kodlar�
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic olan yap�larda type'�n� belli ederiz.
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>)); //generic i�erisinde iki adet entity d�nd��� i�in "<>"
                                                                                       //i�erisinde ka� adet entity varsa virg�l konur.
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //yukar�daki kodlarla constructorlarda ge�mi� oldu�umuz class'lar� DI Container'a ekledik.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"),sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("authserver.data"); //Projenin ismi ne ise Assembly'nin ismi de o olmal�d�r.
                }); //veri taban� yolunu bildirmi� oluyoruz.
            });


            //DI Container'a ekliyoruz
            //Kullan�c�n�n "UserApp" oldu�unu belirtiyoruz, rol� ise identity k�t�phanesinden gelen default role. ek olarak rol eklemek istersek UserApp class�nda
            //ilgili alanda a��klama sat�r� halinde bir �rnek mevcuttur.
            services.AddIdentity<UserApp, IdentityRole>(Opt =>
            {
                Opt.User.RequireUniqueEmail = true; //Email'in veri taban�nda benzersiz olmas� i�in gerekli kod.
                Opt.Password.RequireNonAlphanumeric = false; // '*,?,-' gibi karakterlerin �ifre alan�nda zorunlu olmas�n� engelleyen kod.
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //"AddDefaultTokenProviders" >> �ifre s�f�rlama gibi i�lemlerde yeni bir token 
            //�retmek i�in gerekiyor.


            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //bu kod sat�r�; �stteki kod sat�r� ile alttaki kod sat�r�n�n birbiriyle ileti�im
                //halinde olmas� sa�lamaktad�r.
                //3'�n�n de ismi ayn� olmal�d�r ki birbirleriyle ileti�im halinde olabilsinler.
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0], //audience'lar�n ilk indisini vermemizin sebebi bizim ana projemiz olmas�d�r. ana projeyi vererek di�er
                    //audience'lara da eri�im sa�latabiliriz.
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    //kontrol ediliyor
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //default olarak verilen 5dk'y� s�f�rlar. farkl� serverlarda �al��maktan dolay� olu�an zamanlama hatalar�n� tolere edebilmek
                    //i�in default de�er 5dk'd�r. test ama�l� olmas� nedeniyle bunu 0'a �ekeriz.
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
