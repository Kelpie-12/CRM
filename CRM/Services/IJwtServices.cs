using CRM.Model;

namespace CRM.Services
{
    public interface IJwtServices
    {
        public string GenerateToken (User user);
    }
}
