
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class SalesServices : ISalesServices
    {
        private readonly ISalesRepository _salesRepository;
        public SalesServices(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }
        public async Task<SaleResponse> CreateSales(SaleModel model)
        {
            return await _salesRepository.CreateSales(model);
        }

        public Task<Sales> CreateSalesForClient(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<Task> DeleteSales(int saleId, CompanyDto company)
        {
            return await _salesRepository.DeleteSales(saleId, company);
        }
        public Task<SaleResponse> UpdateSales(SaleModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Sales>> GetAll(CompanyDto company, int status)
        {
            return await _salesRepository.GetAll(company,status);
        }

        public Task<Sales> GetInfoSales(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoSalesByClient(string data)
        {
            throw new NotImplementedException();
        }


        Task<string> ISalesServices.DeleteSales(int saleId, CompanyDto company)
        {
            throw new NotImplementedException();
        }

        public Task<SaleResponse> CancelSales(SaleCancelModel company)
        {
            return _salesRepository.CancelSale(company);
        }
    }
}
