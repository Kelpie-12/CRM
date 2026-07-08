using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface IClientServices
    {
        Task<Client> CreateClient(ClientModel model); 
        Task<Client> UpdateClient(ClientModel model); 
        Task<Task> DeleteClient(ClientModel model);
        Task<bool> CreateSalesForClient(string data); 
        Task<Client> GetInfoClient(InfoClientModel model); 
        Task<string> GetInfoSalesClient(string data); 
        Task<ICollection<Client>> GetInfoListClients(int companyId); 
    }
}
