using CRM.Model;

namespace CRM.Repositories
{
    public interface IUserRepository
    {
        Task<User> Login(string username);
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(User user);
        Task<User> GetUser(string username);
        Task<User> GetUser(int userId,int companyId);

    }
}
