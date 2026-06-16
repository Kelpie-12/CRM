
using CRM.Model;
using CRM.Model.DTO;
using CRM.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CRM.Services.Implementations
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtServices _jwtServices;
        public UserServices(IUserRepository userRepository, IJwtServices jwtServices)
        {
            _jwtServices = jwtServices;
            _userRepository = userRepository;
        }
        public async Task<string> CreateUser(UserDto user)
        {
            user.HashPassword = new PasswordHasher<string>().HashPassword(user.UserName, user.HashPassword);
            //User user = new User()
            //{
            //    UserName = username,
            //    FirstName = firstName,
            //    LastName = lastName,
            //    HashPassword = 
            //};

            if (await _userRepository.CreateUser(user) != null)
            {
                return "Создано";
            }
            return "Не возмжно создать";
        }

        public Task<string> ForgotPassword(string username)
        {


            throw new NotImplementedException();
        }

        public async Task<User> Login(string username, string password)
        {
            User user = await _userRepository.Login(username);
            if (user != null)
            {
                PasswordVerificationResult res = new PasswordHasher<string>().VerifyHashedPassword(username, user.HashPassword, password);
                if (res == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }
            return null;
            //throw  new Exception("неверный логин или пароль");
        }

        public async Task<string> LoginInCompany(string username, string password)
        {

            // string hashPassword=new PasswordHasher<string>().HashPassword(username, password);
            User user = await _userRepository.Login(username);
            PasswordVerificationResult res = new PasswordHasher<string>().VerifyHashedPassword(username, user.HashPassword, password);
            if (res == PasswordVerificationResult.Success)
            {
                return _jwtServices.GenerateToken(user);
            }
            return "Пользователя с таким логином нет";
        }

        public Task<string> Logout()
        {

            throw new NotImplementedException();
        }
    }
}
