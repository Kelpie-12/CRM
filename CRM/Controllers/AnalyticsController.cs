using System.Globalization;
using CRM.Model;
using CRM.Model.DTO;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsServices _analyticsServices;

        public AnalyticsController(IAnalyticsServices analyticsServices)
        {
            _analyticsServices = analyticsServices;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Read)]
        [Route("GetByCompany/{companyId}/{companyName}")]
        public async Task<IActionResult> GetByCompany(
            int companyId,
            string companyName,
            [FromQuery] string? from,
            [FromQuery] string? to)
        {
            if (companyId < 0 || string.IsNullOrWhiteSpace(companyName))
            {
                return BadRequest(new { message = "The wrong company was selected" });
            }

            if (!TryParseQueryDate(from, out DateTime? fromDate) ||
                !TryParseQueryDate(to, out DateTime? toDate))
            {
                return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD." });
            }

            AnalyticsDto analytics = await _analyticsServices.GetAnalytics(
                new CompanyDto { Id = companyId, Name = companyName },
                fromDate,
                toDate);

            return Ok(new { message = "Analytics", response = analytics });
        }

        private static bool TryParseQueryDate(string? value, out DateTime? date)
        {
            date = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            if (DateTime.TryParseExact(
                    value,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime exactDate))
            {
                date = exactDate.Date;
                return true;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                date = parsedDate.Date;
                return true;
            }

            return false;
        }
    }
}