namespace CRM.Repositories
{
    public interface IClientRepository
    {
        Task<bool> CreateClient(string data);
        Task<bool> EditingClient(string data);
        Task<bool> CreateSalesForClient(string data);
        Task<string> GetInfoClient(string data);
        Task<string> GetInfoSalesClient(string data);
        Task<string> GetInfoListClients(string data);
    }
}
