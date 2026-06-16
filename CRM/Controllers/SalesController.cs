using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
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
        [Route("/CreateSales")]
        [Authorize(Policy = Permissions.Create)]
        public async Task<IActionResult> CreateSales([FromBody] CreateSaleModel model)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _salesServices.CreateSales(model));
            }
            return BadRequest("Переданы не верные данные");
        }

       [HttpDelete]
       [Route("/DeleteSales")]
       [Authorize(Policy = Permissions.Delete)]
       public async Task<IActionResult> DeleteSales([FromBody] CompanyDto company,int saleId)
       {          
           if (ModelState.IsValid)
           {
               return Ok(await _salesServices.DeleteSales(saleId,company));
           }
           return BadRequest("Переданы не верные данные");
       }


        [HttpPost]
        [Route("/CreateSalesForClient")]
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
        [Route("/GetInfoListSales")]
        public async Task<string> GetInfoListSales(int id)
        {
            return "InfoListSales item";
        }

    }
}
