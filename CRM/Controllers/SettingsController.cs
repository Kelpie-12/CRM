using CRM.Model;
using CRM.Model.Data;
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
        private readonly ISettingServices _settingServices;

        public SettingController(ISettingServices settingServices)
        {
            _settingServices = settingServices;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Read)]
        [Route("GetInfo/{companyId}/{companyName}")]
        public async Task<IActionResult> GetInfo(int companyId, string companyName)
        {
            return Ok(await _settingServices.GetSettings(companyId, companyName));
        }

        [HttpPut]
        [Authorize(Policy = Permissions.Update)]
        [Route("UpdateCompanyName")]
        public async Task<IActionResult> UpdateCompanyName([FromBody] UpdateCompanySettingsModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName))
            {
                return BadRequest(new { message = "Company name is required." });
            }

            return Ok(new { success = await _settingServices.UpdateCompanyName(model) });
        }

        [HttpPut]
        [Authorize(Policy = Permissions.Update)]
        [Route("UpdateDescription")]
        public async Task<IActionResult> UpdateDescription([FromBody] UpdateCompanySettingsModel model)
        {
            return Ok(new { success = await _settingServices.UpdateCompanyDescription(model) });
        }

        [HttpPut]
        [Authorize(Policy = Permissions.Update)]
        [Route("UpdateCashbox")]
        public async Task<IActionResult> UpdateCashbox([FromBody] UpdateCompanySettingsModel model)
        {
            if (!model.CashboxId.HasValue || model.CashboxId <= 0)
            {
                return BadRequest(new { message = "Cash register is required." });
            }

            return Ok(new { success = await _settingServices.UpdateConnectedCashbox(model) });
        }
    }
}
