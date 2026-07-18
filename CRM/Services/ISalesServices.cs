using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface ISalesServices
    {
        Task<SaleResponse> CreateSales(SaleModel model);
        Task<SaleResponse> UpdateSales(SaleModel model);
        Task<string> DeleteSales(int saleId,CompanyDto company);
        Task<SaleResponse> CancelSales(SaleCancelModel model);
        Task<Sales> CreateSalesForClient(string data);
        Task<Sales> GetInfoSales(string data);
        Task<string> GetInfoSalesByClient(string data);
        Task<ICollection<Sales>> GetAll(CompanyDto company, int status);

    }
}
