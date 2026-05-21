using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRM.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace CRM.Services.Auth
{
    public class JwtServices : IJwtServices
    {
        //private readonly IConfiguration _configuration;
        private readonly IOptions<AuthSettings> _options;
        public JwtServices(/*IConfiguration configuration,*/ IOptions<AuthSettings> options)
        {
            _options = options;
            //   _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            List<Claim> claim = new List<Claim>()
            {
                 new Claim(ClaimTypes.NameIdentifier,user.UserName),
                 new Claim("firstName",user.FirstName),
                 new Claim("lastName",user.LastName),
                // new Claim("selectCompany",user.LastName),
                 new Claim(ClaimTypes.Role,user.Role),
                 new Claim("id",user.Id.ToString())
            };

            JwtSecurityToken jwt = new JwtSecurityToken(
                //expires: DateTime.UtcNow.Add(new TimeSpan()),
                expires: DateTime.UtcNow.Add(_options.Value.Expires),
                claims: claim,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)),
                    SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
            //throw new NotImplementedException();
        }
    }
}
