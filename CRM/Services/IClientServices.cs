using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface IClientServices
    {
        Task<ClientResponse> CreateClient(ClientModel model); 
        Task<ClientResponse> UpdateClient(ClientModel model); 
        Task<Task> DeleteClient(ClientModel model);
        Task<bool> CreateSalesForClient(string data); 
        Task<Client> GetInfoClient(InfoClientModel model); 
        Task<string> GetInfoSalesClient(string data); 
        Task<ICollection<Client>> GetInfoListClients(int companyId, int status); 
    }
}
