using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    // [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v1/[controller]")]
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

        /*
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

         [HttpGet]
         [Route("Task/{id}")]
         public async Task<IActionResult> GetTask(int id)
         {
             if (id == 1) { 

             return Ok(new Tasks() { Id = "1", Title = "task1", IsDone = false });
             }



             return Ok(new Tasks() { Id = "1", Title = "eximple", IsDone = false });
             // Content(res);
         }

         [HttpPost]
         [Route("AddTasks")]
         public async Task<IActionResult> AddTasks([FromForm] Tasks tasks)
         {
             // Tasks tasks = new Tasks() { Id = 1, Title = "test get api", IsDone = false };
             Tasks task = new Tasks();
             //Console.WriteLine(tasks.Title + "Title");
             //Console.WriteLine(tasks.Id + "id");
             task.Id = "3";
             task.Title = "задача побывала на сервре";
             //79-20-21
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

             return Ok(task);
             // Content(res);
         }        

        [HttpGet]
        [Route("Login_test/{username}/{pwd}")]
        public async Task<IActionResult> Login_test(string username,string pwd)
        {
            if (username=="123" && pwd=="123")
            {
                Console.WriteLine($"user-{username}:::pwd-{pwd}");
                return StatusCode(StatusCodes.Status200OK, new { message = "User login" });
            }

            return StatusCode(StatusCodes.Status409Conflict,
                new { message = "User with this email already registered", error = "email_already_exist" });
        }*/
        [HttpPost]
        [Route("CreateUser")]//регистрация
        public async Task<IActionResult> CreateUser([FromBody] UserDto user/*string username, string password, string firstName, string lastName*/)
        {
            //Console.WriteLine(user);
            //Console.WriteLine(user.UserName);
            //Console.WriteLine();
            UserDto res = await _userServices.CreateUser(user);
            if (res.Id != -1)
            {
                return CreatedAtAction(nameof(CreateUser), new { message = "User created" }, res);
            }
            return StatusCode(StatusCodes.Status409Conflict,
              new { message = "User with this email already registered", error = "email_already_exist" });

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login/*string username, string password*/)
        {
            //Console.WriteLine();
            //Console.WriteLine($"{login.Username}  {login.Password}");
            //Console.WriteLine();
            User user = await _userServices.Login(login.Username, login.Password);
            user.Role = "-1";
            if (user.Id==-1)
            {
                //return Unauthorized();
                return BadRequest(new { message = "Неверные данные" });
            }
            string companies = await _companyServices.GetAllByUser(user);
            //возвращать токен и список компаний для конкретного юзера
            return Content(companies);
        }

      
        //перенести в контроллер компаний
        //[HttpGet]
        //[Route("LoginCompany/{companyId}/{userId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> LoginCompany(int companyId,int userId)
        //{
        //    LoginCompanyModel company = await _companyServices.GetLoginCompany(companyId, userId);

        //    //должен передавать все данные о компании, главную страницу назавния и тд.
        //    //юзера с определенной ролью

        //    return Ok(company);
        //}

        //[HttpPost]
        //[Route("CreateCompany")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> CreateCompany(CompanyModel model)
        //{
        //    //доделать таблицу токенов с их статусом, добваить возможность админу идалять пользователей, создавайть новых и наделять правами и ролями
        //    string res = await _companyServices.CreateCompany(model.Company, model.User);

        //    return Ok(res);
        //}


    }

}
