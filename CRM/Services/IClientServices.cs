using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface IClientServices
    {
        Task<Client> CreateClient(CreateClientModel model); 
        Task<Client> EditingClient(CreateClientModel model); 
        Task<bool> CreateSalesForClient(string data); 
        Task<Client> GetInfoClient(ClientDto client); 
        Task<string> GetInfoSalesClient(string data); 
        Task<string> GetInfoListClients(string data); 
    }
}
