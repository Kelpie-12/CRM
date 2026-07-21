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
    public class EmployeeController : Controller
    {
        private readonly IEmployeeServices _employeeServices;

        public EmployeeController(IEmployeeServices employeeServices)
        {
            _employeeServices = employeeServices;
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Create)]
        [Route("Create")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeModel model)
        {
            EmployeeResponse response = await _employeeServices.CreateEmployee(model);
            if (response.Employee?.Id != -1)
            {
                return CreatedAtAction(nameof(CreateEmployee), response);
            }

            return StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPut]
        [Authorize(Policy = Permissions.Update)]
        [Route("Update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeModel model)
        {
            var response = await _employeeServices.UpdateEmployee(model);
            if (response.Employee?.Id != -1)
            {
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpDelete]
        [Authorize(Policy = Permissions.Delete)]
        [Route("Delete")]
        public async Task<IActionResult> DeleteEmployee([FromBody] EmployeeModel model)
        {
            if (_employeeServices.DeleteEmployee(model).IsCompleted)
            {
                return Ok(new { message = "Employee was deleted" });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "", error = "deleted_employee_error" });
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Read)]
        [Route("GetAll/{companyId}/{companyName}")]
        public async Task<IActionResult> GetAll(int companyId, string companyName, [FromQuery] int status = Status.Active)
        {
            if (companyId < 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "The wrong company was selected" });
            }

            return Ok(new
            {
                message = "All employees",
                response = await _employeeServices.GetAll(new CompanyDto { Id = companyId, Name = companyName }, status)
            });
        }
    }
}
