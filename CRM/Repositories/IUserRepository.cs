using CRM.Model;

namespace CRM.Repositories
{
    public interface IUserRepository
    {
        Task<User> Login(string username);
        Task<User> CreateUser(User user);

    }
}
