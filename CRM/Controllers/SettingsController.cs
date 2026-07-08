using CRM.Model;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SettingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISettingServices _settingServices;
        public SettingController(IConfiguration configuration,ISettingServices settingServices)
        {
            _configuration = configuration;
            _settingServices = settingServices;
        }

        [HttpPost]
        [Route("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("EditingEmployee")]
        public async Task<IActionResult> EditingEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("RecoveryEmployee")]
        public async Task<IActionResult> RecoveryEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("EditingInfoAboutEnterprise")]
        public async Task<IActionResult> EditingInfoAboutEnterprise(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("EditingSettings")]
        public async Task<IActionResult> EditingSettings(string data)
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetInfoListEmployee")]
        public async Task<string> GetInfoListSales(int id)
        {
            return "InfoList Employee item";
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Read)]
        [Route("GetInfo/{id}")]
        public async Task<IActionResult> GetInfo(int id)
        {
            Console.WriteLine(Request.HttpContext.Request.Headers);
            return Ok(" Get Info Setting item");
        }


        [HttpGet]
        [Route("GetInfoAboutEnterprise")]
        public async Task<string> GetInfoAboutEnterprise(int id)
        {
            return " Get Info About Enterprise item";
        }
    }
}
