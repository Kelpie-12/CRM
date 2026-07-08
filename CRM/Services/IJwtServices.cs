using CRM.Model;

namespace CRM.Services
{
    public interface IJwtServices
    {
        public string GenerateToken (User user);
        public string GenerateTokenLoginCompany(User user,string selectedCompany);
    }
}
