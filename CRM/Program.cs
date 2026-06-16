using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using CRM.Services;
using CRM.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CRM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(cont =>
            {
                Assembly assembly = typeof(Program).Assembly;
                cont.RegisterAssemblyTypes(assembly)
                                    .Where(t => t.Name.EndsWith("Services"))
                                    .AsImplementedInterfaces()
                                    .SingleInstance();
            });
            builder.Host.ConfigureContainer<ContainerBuilder>(cont =>
            {
                Assembly assembly = typeof(Program).Assembly;
                cont.RegisterAssemblyTypes(assembly)
                                    .Where(t => t.Name.EndsWith("Repository"))
                                    .AsImplementedInterfaces()
                                    .SingleInstance();
            });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
            builder.Services.AddAuth(builder.Configuration);

            var MyAllowSpecificOrigins = "_myCors";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                     policy =>
                     {
                         policy
                         .WithOrigins("http://localhost:5173") // Your frontend URL
                         .AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                         //.WithMethods("PUT", "DELETE", "GET","POST");
                     });
            });

            /*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(o => {
            //        o.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuer = false,
            //            ValidateAudience = false,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("securetikeyasdasdasdasdasdasdzxcfgdshdfs"))
            //        };
               });*/
            //builder.WebHost.UseUrls("http://localhos:8080");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // app.UseHttpsRedirection();

            app.UseRouting();
            app.Use(async (context, next) =>
            {
                //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.AccessControlAllowOrigin = "*";
                context.Response.Headers.AccessControlAllowMethods = "GET, POST, PUT, DELETE, OPTIONS";
                context.Response.Headers.AccessControlAllowHeaders = "Content-Type, Authorization";
            
                await next();
            });
            app.UseCors(MyAllowSpecificOrigins);
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseAuthentication();
        


            app.MapControllers();

            app.Run();
        }
    }
}
