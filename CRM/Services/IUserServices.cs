using CRM.Model;

namespace CRM.Services
{
    public interface IUserServices
    {
        Task<User> Login(string username, string password);
        Task<string> LoginInCompany(string username, string password);
        Task<string> Logout();
        Task<string> CreateUser(string username, string password, string firstName, string lastName);
        Task<string> ForgotPassword(string username);

    }
}
