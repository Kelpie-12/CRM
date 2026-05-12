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
        public async Task<IActionResult> CreateSales(string data)
        {           

            return Ok();
        }

        [HttpPost]
        [Route("/EditingSales")]
        public async Task<IActionResult> EditingSales(string data)
        {
            return Ok();
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
