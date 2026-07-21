using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface ITransactionRepository
    {
        Task<ICollection<Transaction>> GetAll(CompanyDto company, int status);
        Task<FiscalDocumentDto?> GetFiscalDocument(CompanyDto company, int fiscalDocumentId);
    }
}
