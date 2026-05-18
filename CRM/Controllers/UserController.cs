using System.Text.Json;
using CRM.Model;
using CRM.Model.Data;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        private readonly ICompanyServices _companyServices;
        public UserController(IUserServices userServices, ICompanyServices companyServices)
        {
            _userServices = userServices;
            _companyServices = companyServices;
        }

        [HttpPost]
        [Route("CreateUser")]//регистрация
        public async Task<IActionResult> CreateUser(string username, string password, string firstName, string lastName)
        {
            string res = await _userServices.CreateUser(username, password, firstName, lastName);
            return Content(res);
        }

        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            User user = await _userServices.Login(username, password);
            if (user == null)
            {
                //return Unauthorized();
                return BadRequest(new { message = "Неверные данные" });
            }
            string companies = await _companyServices.GetAllByUser(user);
            //возвращать токен и список компаний для конкретного юзера
            return Content(companies);
        }

        [HttpGet]
        [Route("LoginCompany")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LoginCompany(int companyId, int userId)
        {
            LoginCompanyModel company = await _companyServices.GetById(companyId, userId);
            //должен передавать все данные о компании, главную страницу назавния и тд.
            //юзера с определенной ролью

            return Ok(company);
        }

        [HttpPost]
        [Route("CreateCompany")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyModel model)
        {
            //доделать таблицу токенов с их статусом, добваить возможность админу идалять пользователей, создавайть новых и наделять правами и ролями
            string res = await _companyServices.CreateCompany(model.Company, model.User);

            return Ok(res);
        }


    }
}
