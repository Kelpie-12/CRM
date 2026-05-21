using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IClientRepository
    {
        Task<Client> CreateClient(CompanyDto company, ClientDto client,UserDto user);
        Task<Client> EditingClient(CompanyDto company, ClientDto client, UserDto user);
        Task<Sales> CreateSalesForClient(string data);
        Task<Client> GetInfoClient(ClientDto client);
        Task<ICollection<Sales>> GetInfoSalesClient(string data);
        Task<ICollection<Client>> GetInfoListClients(string data);
    }
}
