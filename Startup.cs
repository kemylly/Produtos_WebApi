using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using estudo_api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
//novas
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace estudo_api
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
           
            services.AddControllers();

            //Dizer que estamos utilizando as confiuracoes jwt no token
            string chaveDeSegurana = "kemylly_cavalcante_santos"; //chave de seguranca do seu token
            var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSegurana));
            // vamos usar a jwt como autenticacao
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>{
                //como o sistema deve ler o nosso token
                options.TokenValidationParameters = new TokenValidationParameters{ //diz se um token é valido ou não em um sistema
                    //
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    //dados de validadcao de um jwt
                    ValidIssuer = "supermecado.com",
                    ValidAudience = "usuario_comum", 
                    IssuerSigningKey = chaveSimetrica
                };
            });

             //swagger
            services.AddSwaggerGen(config => {
                config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {Title="API DE PRODUTOS", Version = "v1"});
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // aplica o sistema de autenticacao na sua aplicacao

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //documentacao por meio do swagger
            //app.UseSwagger(); // gerar um arquivo json com rota padrao - swagger.json
            app.UseSwagger(config => { //rota modificada
                config.RouteTemplate = "kemylly/{documentName}/swagger.json";
            }); 
            app.UseSwaggerUI(config => { //usar as views html do swagger
                //config.SwaggerEndpoint("/swagger/v1/swagger.json","v1 docs");  //rota padrao
                config.SwaggerEndpoint("/kemylly/v1/swagger.json","v1 docs"); //rota modificada
            });
        }
    }
}
