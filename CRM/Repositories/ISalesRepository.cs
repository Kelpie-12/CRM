namespace CRM.Repositories
{
    public interface ISalesRepository
    {
        Task<bool> CreateSales(string data);
        Task<bool> EdiningSales(string data);
        Task<bool> CreateSalesForClient(string data);
        Task<string> GetInfoSales(string data);
        Task<string> GetInfoSalesByClient(string data);
        Task<string> GetInfoListSales(string data);
    }
}
