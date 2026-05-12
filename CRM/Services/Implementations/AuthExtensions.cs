using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Services.Implementations
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth(
            this IServiceCollection servicesCollection,
            IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
            servicesCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters =  new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey))
                    };
                });

            return servicesCollection;
        }
    }
}
