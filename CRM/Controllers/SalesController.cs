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
    public class SalesController : Controller
    {
        private readonly ISalesServices _salesServices;
        public SalesController(ISalesServices salesServices)
        {
            _salesServices = salesServices;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Policy = Permissions.Create)]
        public async Task<IActionResult> CreateSales([FromBody] SaleModel model)
        {
            if (ModelState.IsValid)
            {
                SaleResponse response = await _salesServices.CreateSales(model);
                if (response.Sales.Id != -1)
                {
                    return StatusCode(StatusCodes.Status201Created, response);
                }
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }
            return BadRequest(new { message = "Incorrect data! Check data", error = "error_incorrect_data" });
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize(Policy = Permissions.Delete)]
        public async Task<IActionResult> DeleteSales([FromBody] CompanyDto company, int saleId)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _salesServices.DeleteSales(saleId, company));
            }
            return BadRequest("Переданы не верные данные");
        }

        [HttpPut]
        [Route("Cancel")]
        [Authorize(Policy = Permissions.Update)]
        public async Task<IActionResult> CancelSales([FromBody]SaleCancelModel sale )
        {
            if (ModelState.IsValid)
            {
                return Ok(await _salesServices.CancelSales(sale));
            }
            return BadRequest(new { message = "Incorrect data! Check data", error = "error_incorrect_data" });
        }

        [HttpPost]
        [Route("CreateSalesForClient")]
        public async Task<IActionResult> CreateSalesForClient(string data)
        {
            return Ok();
        }

        [HttpGet]
        [Route("/GetInfoSales")]
        public async Task<string> GetInfoSales(int id)
        {
            return "InfoSales item";
        }

        [HttpGet]
        [Route("/GetInfoSalesByClient")]
        public async Task<string> GetInfoSalesByClient(int id)
        {
            return "InfoSalesByClient item";
        }

        [HttpGet]
        [Route("GetAll/{companyId}/{companyName}")]
        public async Task<IActionResult> GetAll(int companyId, string companyName,[FromQuery]int status)
        {
            return StatusCode(StatusCodes.Status200OK,
                await _salesServices.GetAll(new CompanyDto() { Id = companyId, Name = companyName },status));
        }

    }
}
