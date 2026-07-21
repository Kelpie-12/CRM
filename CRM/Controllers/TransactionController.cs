using CRM.Model;
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
    public class TransactionController : Controller
    {
        private readonly ITransactionServices _transactionServices;

        public TransactionController(ITransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Read)]
        [Route("GetAll/{companyId}/{companyName}")]
        public async Task<IActionResult> GetAll(int companyId, string companyName, [FromQuery] int status = Status.Active)
        {
            if (companyId < 0)
            {
                return BadRequest(new { message = "The wrong company was selected" });
            }

            return Ok(new
            {
                message = "All transactions",
                response = await _transactionServices.GetAll(
                    new CompanyDto { Id = companyId, Name = companyName },
                    status)
            });
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Read)]
        [Route("GetFiscalDocument/{companyId}/{companyName}/{fiscalDocumentId}")]
        public async Task<IActionResult> GetFiscalDocument(int companyId, string companyName, int fiscalDocumentId)
        {
            if (companyId < 0 || fiscalDocumentId <= 0)
            {
                return BadRequest(new { message = "Invalid request parameters." });
            }

            FiscalDocumentDto? document = await _transactionServices.GetFiscalDocument(
                new CompanyDto { Id = companyId, Name = companyName },
                fiscalDocumentId);

            if (document == null)
            {
                return NotFound(new { message = "Fiscal document not found." });
            }

            return Ok(new { message = "Fiscal document", response = document });
        }
    }
}
