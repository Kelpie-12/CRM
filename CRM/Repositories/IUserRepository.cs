using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IUserRepository
    {
        //Task<ICollection<Permission>> GetPermissions(string userName);
        Task<User> Login(string username);
        Task<UserDto> CreateUser(UserDto user);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(User user);
        Task<User> GetUser(string username);
        Task<User> GetUser(int userId,int companyId);

    }
}
