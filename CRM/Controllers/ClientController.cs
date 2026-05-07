using CRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [Route("/CreateClient")]
        public async Task<IActionResult> CreateClient(string name, string data)
        {
            if (await _clientServices.CreateClient(data))
            {
                return Ok();
            }
            return BadRequest("Wrong!");

        }
        [HttpPost]
        [Route("/EditingClient")]
        public async Task<IActionResult> EditingClient(string name, string data)
        {
            return Ok();
        }
   
        [HttpPost]
        [Route("/CreateSalesForClientByManager")]
        public async Task<IActionResult> CreateSalesForClient(string data)
        {

            return Ok();
        }

        [HttpGet]
        [Route("/GetInfoClient")]
        public async Task<string> GetInfoClient(int id)
        {
            return "InfoSalesClient item";
        }

        [HttpGet]
        [Route("/GetInfoSalesClient")]
        public async Task<string> GetInfoSalesClient(int id)
        {
            return "InfoSalesClient item";
        }

        [HttpGet]
        [Route("/GetInfoListClients")]
        public async Task<string> GetInfoListClients(int id)
        {
            return "InfoClient List item";
        }
    }
}
