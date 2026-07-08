
using CRM.Model;
using CRM.Model.DTO;
using CRM.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<UserDto> CreateUser(UserDto user)
        {
            user.Password = new PasswordHasher<string>().HashPassword(user.UserName, user.Password);

            return await _userRepository.CreateUser(user);
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
            return new User() { Id=-1};
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
