using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Reflection;

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
            //builder.WebHost.UseUrls("http://localhos:8080");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
