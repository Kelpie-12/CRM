using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class PurchaseServices : IPurchaseServices
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseServices(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public Task<PurchaseResponse> CreatePurchase(PurchaseModel model)
        {
            return _purchaseRepository.CreatePurchase(model);
        }

        public Task<Task> DeletePurchase(int purchaseId, CompanyDto company)
        {
            return _purchaseRepository.DeletePurchase(purchaseId, company);
        }

        public Task<ICollection<Purchase>> GetAll(CompanyDto company, int status)
        {
            return _purchaseRepository.GetAll(company, status);
        }
    }
}
