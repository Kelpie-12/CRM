using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseServices _purchaseServices;

        public PurchaseController(IPurchaseServices purchaseServices)
        {
            _purchaseServices = purchaseServices;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Policy = Permissions.Create)]
        public async Task<IActionResult> CreatePurchase([FromBody] PurchaseModel model)
        {
            PurchaseResponse response = await _purchaseServices.CreatePurchase(model);
            if (response.Purchase?.Id != -1)
            {
                return StatusCode(StatusCodes.Status201Created, response);
            }

            return StatusCode(StatusCodes.Status400BadRequest, response);
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize(Policy = Permissions.Delete)]
        public async Task<IActionResult> DeletePurchase([FromBody] CompanyDto company, int purchaseId)
        {
            await _purchaseServices.DeletePurchase(purchaseId, company);
            return Ok(new { message = "Purchase was deleted" });
        }

        [HttpGet]
        [Route("GetAll/{companyId}/{companyName}")]
        [Authorize(Policy = Permissions.Read)]
        public async Task<IActionResult> GetAll(int companyId, string companyName, [FromQuery] int status = Status.Active)
        {
            return Ok(await _purchaseServices.GetAll(
                new CompanyDto { Id = companyId, Name = companyName },
                status));
        }
    }
}
