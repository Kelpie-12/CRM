using System.Text;
using CRM.Model;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Services.Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth(
            this IServiceCollection servicesCollection,
            IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
            servicesCollection.AddSingleton<IAuthorizationHandler, PermissionRequirementsHandler>();
            servicesCollection
                .AddAuthorization(x=>
                {
                    x.AddPolicy(Permissions.Read,
                        policy => policy.Requirements.Add(new PermissionRequirements(Permissions.Read)));
                    x.AddPolicy(Permissions.Delete,
                        policy => policy.Requirements.Add(new PermissionRequirements(Permissions.Delete)));
                    x.AddPolicy(Permissions.Update,
                      policy => policy.Requirements.Add(new PermissionRequirements(Permissions.Update)));
                    x.AddPolicy(Permissions.Create,
                   policy => policy.Requirements.Add(new PermissionRequirements(Permissions.Create)));
                }
                )
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
