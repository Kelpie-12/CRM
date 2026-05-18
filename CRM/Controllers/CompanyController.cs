using CRM.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompanyController : Controller
    {
        [HttpGet]
        [Route("GetCompanyById")]
        public IActionResult GetCompanyById(int id)
        {
            return View();
        }

        [HttpGet]
        [Route("GetCompanyForUser")]
        public IActionResult GetCompanyForUser(User user)
        {
            return View();
        }
        [HttpPost]
        [Route("CreateCompany")]
        public IActionResult CreateCompany(Company company)
        {


            return View();
        }
        [HttpPost]
        [Route("SetRole")]
        public IActionResult SetRole(User user)
        {
            return View();
        }




    }
}
