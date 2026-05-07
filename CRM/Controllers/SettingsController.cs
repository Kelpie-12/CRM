using CRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        private readonly ISettingServices _settingServices;
        public SettingsController(ISettingServices settingServices)
        {
            _settingServices = settingServices;
        }

        [HttpPost]
        [Route("/CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("/EditingEmployee")]
        public async Task<IActionResult> EditingEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("/DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("/RecoveryEmployee")]
        public async Task<IActionResult> RecoveryEmployee(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("/EditingInfoAboutEnterprise")]
        public async Task<IActionResult> EditingInfoAboutEnterprise(string data)
        {
            return Ok();
        }

        [HttpPost]
        [Route("/EditingSettings")]
        public async Task<IActionResult> EditingSettings(string data)
        {
            return Ok();
        }

        [HttpGet]
        [Route("/GetInfoListEmployee")]
        public async Task<string> GetInfoListSales(int id)
        {
            return "InfoList Employee item";
        }

        [HttpGet]
        [Route("/GetInfoSetting")]
        public async Task<string> GetInfoSetting(int id)
        {
            return " Get Info Setting item";
        }


        [HttpGet]
        [Route("/GetInfoAboutEnterprise")]
        public async Task<string> GetInfoAboutEnterprise(int id)
        {
            return " Get Info About Enterprise item";
        }
    }
}
