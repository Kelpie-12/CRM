using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface IUserServices
    {
        Task<User> Login(string username, string password);
        Task<string> LoginInCompany(string username, string password);
        Task<string> Logout();
        Task<string> CreateUser(UserDto user);
        Task<string> ForgotPassword(string username);

    }
}
