
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TasksController : Controller
    {
        private readonly ITaskServices _taskServices;

        public TasksController(ITaskServices taskServices)
        {
            _taskServices = taskServices;
        }

        [HttpPost]
        [Route("CreateTask")]
        public async Task<IActionResult> CreateTask(string data)
        {

            return Content("Create task");
        }

        [HttpGet]
        [Route("GetListTasks")]
        public async Task<IActionResult> GetListTasks(int id, string data)
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetTask")]
        public async Task<IActionResult> GetTask(int id)
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetTaskListByEmployee")]
        public async Task<IActionResult> GetTaskListByEmployee(int id, string nameEmployee)
        {
            return Ok();
        }
    }
}
