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
    public class ClientController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IClientServices _clientServices;
        public ClientController(IClientServices clientServices, IConfiguration configuration)
        {
            _clientServices = clientServices;
            _configuration = configuration;
        }


        [HttpPost]
        [Authorize(Policy = Permissions.Create)]
        [Route("/CreateClient")]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientModel model)
        {
            Client res = new Client();
            res = await _clientServices.CreateClient(model);
            return Json(res);

        }
        [HttpPost]
        [Authorize(Policy = Permissions.Edit)]
        [Route("/EditClient")]
        public async Task<IActionResult> EditClient([FromBody] CreateClientModel model)
        {
            Client res = new Client();
            res = await _clientServices.CreateClient(model);
            return Json(res);
        }

        [HttpPost]
        [Route("/CreateSalesForClientByEmployee")]
        public async Task<IActionResult> CreateSalesForClientByEmployee(string data)
        {

            return Ok();
        }

        [HttpGet]
        [Route("/GetInfoClient")]
        [Authorize(Policy = Permissions.Read)]
        public async Task<IActionResult> GetInfoClient([FromBody] InfoClientModel model)
        {
           Client res = new Client();
            res = await _clientServices.GetInfoClient(model);

            return Json(res);
        }

        [HttpGet]
        [Route("/GetInfoSalesClient")]
        public async Task<string> GetInfoSalesClient(int id)
        {
            return "InfoSalesClient item";
        }

        [HttpGet]
        [Route("/GetInfoListClients")]
        [Authorize(Policy = Permissions.Read)]
        public async Task<IActionResult> GetInfoListClients([FromBody] CompanyDto company)
        {
            //проверки подлиности запросов

            return Ok(_clientServices.GetInfoListClients(company));
        }
    }
}
