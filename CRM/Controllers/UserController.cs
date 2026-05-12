using CRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        [Route("CreateUser")]//регистрация
        public async Task<IActionResult> CreateUser(string username, string password, string firstName, string lastName)
        {
          string res=  await _userServices.CreateUser(username, password, firstName, lastName);
            return Content(res);
        }

        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            string res = await _userServices.Login(username, password);
            return Content(res);
        }


    }
}
