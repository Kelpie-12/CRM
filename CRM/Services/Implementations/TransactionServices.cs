using CRM.Model;
using CRM.Model.DTO;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class TransactionServices : ITransactionServices
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionServices(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public Task<ICollection<Transaction>> GetAll(CompanyDto company, int status)
        {
            return _transactionRepository.GetAll(company, status);
        }

        public Task<FiscalDocumentDto?> GetFiscalDocument(CompanyDto company, int fiscalDocumentId)
        {
            return _transactionRepository.GetFiscalDocument(company, fiscalDocumentId);
        }
    }
}
