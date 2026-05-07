
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<string> CreateUser(string username, string password, string firstName, string lastName)
        {
            string res = await _userRepository.CreateUser(username, password, firstName, lastName);
            return res;
        }

        public Task<string> ForgotPassword(string username)
        {


            throw new NotImplementedException();
        }

        public async Task<string> Login(string username, string password)
        {
            return await _userRepository.Login(username, password);
        }

        public Task<string> Logout()
        {

            throw new NotImplementedException();
        }
    }
}
