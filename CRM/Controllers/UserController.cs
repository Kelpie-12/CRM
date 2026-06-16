using System.Text.Json;

using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CRM.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
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

        [HttpGet]
        [Route("Tasks")]
        public async Task<IActionResult> GetTasks()
        {
            // Tasks tasks = new Tasks() { Id = 1, Title = "test get api", IsDone = false };
            List<Tasks> tasks = new List<Tasks>()
            {
                    new Tasks(){ Id = "1", Title = "test get api", IsDone = false },
                    new Tasks(){ Id = "2", Title = "test get api", IsDone = true }
            };
            //string res = System.Text.Json.JsonSerializer.Serialize(tasks,new JsonSerializerOptions()
            //{
            //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            //    WriteIndented = true
            //});

            return Ok(tasks);
            // Content(res);
        }
        [HttpPost]
        [Route("AddTasks")]
        public async Task<IActionResult> AddTasks([FromForm] Tasks tasks)
        {
            // Tasks tasks = new Tasks() { Id = 1, Title = "test get api", IsDone = false };
            Tasks task=new Tasks();
            Console.WriteLine(tasks.Title +"Title");
            Console.WriteLine(tasks.Id+"id");
            task.Id ="3";
            task.Title = "задача побывала на сервре";

            List<Tasks> tasks1 = new List<Tasks>()
            {
                task,
                new Tasks(){Id="43",Title="ttt",IsDone=true}
                  //new Tasks(){ Id = "1", Title = "test get api", IsDone = false },
                  //new Tasks(){ Id = "2", Title = "test get api", IsDone = true }
            };
            //string res = System.Text.Json.JsonSerializer.Serialize(tasks, new JsonSerializerOptions()
            //{
            //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            //    WriteIndented = true
            //});

            return Ok(tasks1);
            // Content(res);
        }


        [HttpPost]
        [Route("CreateUser")]//регистрация
        public async Task<IActionResult> CreateUser([FromForm] UserDto user/*string username, string password, string firstName, string lastName*/)
        {
            string res = await _userServices.CreateUser(user);
            return Content(res);
        }

        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            User user = await _userServices.Login(username, password);
            user.Role = "-1";
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
        public async Task<IActionResult> CreateCompany(CreateCompanyModel model)
        {
            //доделать таблицу токенов с их статусом, добваить возможность админу идалять пользователей, создавайть новых и наделять правами и ролями
            string res = await _companyServices.CreateCompany(model.Company, model.User);

            return Ok(res);
        }


    }
    public class Tasks
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("isDone")]
        public bool IsDone { get; set; }
    }
}
