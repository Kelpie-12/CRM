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
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompanyController : Controller
    {
        private readonly ICompanyServices _companyServices;
        public CompanyController(ICompanyServices companyServices)
        {
            _companyServices = companyServices;
        }    

        [HttpGet]
        [Route("Login/{companyId}/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LoginCompany(int companyId, int userId)
        {
            LoginCompanyModel company = await _companyServices.GetLoginCompany(companyId, userId);

            //должен передавать все данные о компании, главную страницу назавния и тд.
            //юзера с определенной ролью

            return Ok(company);
        }
        [HttpPost]
        [Route("Create")]
        //[Authorize(Roles =Roles.Admin)]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyModel company)
        {
            CreateCompanyResponse response = await _companyServices.CreateCompany(company.Company,company.User);
            if (response.Company.Id== -1)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new { message = response.Message, error = response.Error, company=response.Company});
                
            }
            return StatusCode(StatusCodes.Status201Created,response);
            

            //return StatusCode(StatusCodes.Status201Created);
        }


    }
}
