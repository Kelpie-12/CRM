using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IPurchaseRepository
    {
        Task<PurchaseResponse> CreatePurchase(PurchaseModel model);
        Task<Task> DeletePurchase(int purchaseId, CompanyDto company);
        Task<ICollection<Purchase>> GetAll(CompanyDto company, int status);
    }
}
