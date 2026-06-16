using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface ISalesServices
    {
        Task<string> CreateSales(CreateSaleModel model);
        Task<string> DeleteSales(int saleId,CompanyDto company);
        Task<Sales> CreateSalesForClient(string data);
        Task<Sales> GetInfoSales(string data);
        Task<string> GetInfoSalesByClient(string data);
        Task<string> GetInfoListSales(string data);

    }
}
