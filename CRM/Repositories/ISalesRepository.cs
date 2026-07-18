using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface ISalesRepository
    {
        Task<SaleResponse> CreateSales(SaleModel model);
        Task<SaleResponse> UpdateSales(SaleModel model);
        Task<Task> DeleteSales(int saleId, CompanyDto company);
        Task<SaleResponse> CancelSale(SaleCancelModel sale);
        Task<bool> CreateSalesForClient(string data);
        Task<string> GetInfoSales(string data);
        Task<string> GetInfoSalesByClient(string data);
        Task<ICollection<Sales>> GetAll(CompanyDto company, int status);
    }
}
