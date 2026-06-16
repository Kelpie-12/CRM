using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface ISalesRepository
    {
        Task<string> CreateSales(CreateSaleModel model);
        Task<string> DeleteSales(int saleId, CompanyDto company);
        Task<bool> CreateSalesForClient(string data);
        Task<string> GetInfoSales(string data);
        Task<string> GetInfoSalesByClient(string data);
        Task<string> GetInfoListSales(string data);
    }
}
