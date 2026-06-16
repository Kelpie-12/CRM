
using CRM.Model;
using CRM.Model.Data;
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
        public async Task<string> CreateSales(CreateSaleModel model)
        {


            return await _salesRepository.CreateSales(model);

        }

        public Task<Sales> CreateSalesForClient(string data)
        {
            throw new NotImplementedException();
        }

      public async  Task<string> DeleteSales(int saleId, CompanyDto company)
        {
            return await _salesRepository.DeleteSales(saleId,company);
        }

        public Task<string> GetInfoListSales(string data)
        {
            throw new NotImplementedException();
        }

        public Task<Sales> GetInfoSales(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoSalesByClient(string data)
        {
            throw new NotImplementedException();
        }
    }
}
