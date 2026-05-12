using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AnalyticsController : Controller
    {
        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            return Content("Тест авторизации");
        }
    }
}
