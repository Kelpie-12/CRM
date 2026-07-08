using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IClientRepository
    {
        Task<Client> CreateClient(CompanyDto company, ClientDto client,UserDto user);
        Task<Client> UpdateClient(CompanyDto company, ClientDto client, UserDto user);
        Task<Task> DeleteClient(CompanyDto company, ClientDto client, UserDto user);
        Task<Client> CreateSalesForClient(string data);
        Task<Client> GetInfoClient(InfoClientModel model);
        Task<ICollection<Sales>> GetInfoSalesClient(string data);
        Task<ICollection<Client>> GetInfoListClients(int companyId);
    }
}
