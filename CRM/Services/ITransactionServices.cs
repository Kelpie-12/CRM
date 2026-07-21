using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface ITransactionServices
    {
        Task<ICollection<Transaction>> GetAll(CompanyDto company, int status);
        Task<FiscalDocumentDto?> GetFiscalDocument(CompanyDto company, int fiscalDocumentId);
    }
}
