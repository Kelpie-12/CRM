using System.Web.Http.Filters;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/v1/[controller]")]
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
        [Route("Create")]
        public async Task<IActionResult> CreateClient([FromBody] ClientModel model)
        {
            Client res = new Client();
            res = await _clientServices.CreateClient(model);
            if (res.Id != -1)
            {
                return CreatedAtAction(nameof(CreateClient), new { message = "Client created" }, res);
            }
            return StatusCode(StatusCodes.Status409Conflict,
              new { message = "Client with this last name and first name already registered", error = "client_already_exist" });


        }
        [HttpPut]
        [Authorize(Policy = Permissions.Update)]
        [Route("Update")]
        public async Task<IActionResult> UpdateClient([FromBody] ClientModel model)
        {
            Client res = new Client();
            res = await _clientServices.UpdateClient(model);
            return StatusCode(StatusCodes.Status200OK, new { message = "Client was update" });

        }
        [HttpDelete]
        [Authorize(Policy = Permissions.Delete)]
        [Route("Delete")]
        public async Task<IActionResult> DeleteClient([FromBody] ClientModel model)
        {
            if (_clientServices.DeleteClient(model).IsCompleted)
            {
                return StatusCode(StatusCodes.Status200OK, new { message = "Client was deleted" });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "", error = "deleted_client_error" });

        }

        [HttpPost]
        [Route("/CreateSalesForClientByEmployee")]
        public async Task<IActionResult> CreateSalesForClientByEmployee(string data)
        {

            return Ok();
        }

        [HttpGet]
        [Route("GetAllClientInfo/{companyId}")]
        [Authorize(Policy = Permissions.Read)]
        public async Task<IActionResult> GetAllClientInfo(int companyId)
        {
            InfoClientModel model = new InfoClientModel()
            {
                Client = new ClientDto() { Id = -1 },
                Company = new CompanyDto() { Id = companyId },

            };
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
        [Route("GetAll/{companyId}")]
        [Authorize(Policy = Permissions.Read)]
        public async Task<IActionResult> GetAll(int companyId)
        {
            //проверки подлиности запросов

            return Ok(await _clientServices.GetInfoListClients(companyId));
        }
    }
}
