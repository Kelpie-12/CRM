using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IClientRepository
    {
        Task<ClientResponse> CreateClient(CompanyDto company, ClientDto client,UserDto user);
        Task<ClientResponse> UpdateClient(CompanyDto company, ClientDto client, UserDto user);
        Task<Task> DeleteClient(CompanyDto company, ClientDto client, UserDto user);
        Task<Client> CreateSalesForClient(string data);
        Task<Client> GetInfoClient(InfoClientModel model);
        Task<ICollection<Sales>> GetInfoSalesClient(string data);
        Task<ICollection<Client>> GetInfoListClients(int companyId, int status);
    }
}
