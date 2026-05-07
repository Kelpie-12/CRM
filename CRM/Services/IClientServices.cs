namespace CRM.Services
{
    public interface IClientServices
    {
        Task<bool> CreateClient(string data); 
        Task<bool> EditingClient(string data); 
        Task<bool> CreateSalesForClient(string data); 
        Task<string> GetInfoClient(string data); 
        Task<string> GetInfoSalesClient(string data); 
        Task<string> GetInfoListClients(string data); 
    }
}
