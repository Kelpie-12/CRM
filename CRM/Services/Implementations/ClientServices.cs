
using System.Text.Json;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class ClientServices : IClientServices
    {
        private readonly IClientRepository _clientRepository;
        public ClientServices(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public Task<Client> CreateClient(CreateClientModel model)
        {           
            return _clientRepository.CreateClient(model.Company, model.Client,model.User);
        }

        public Task<bool> CreateSalesForClient(string data)
        {
            throw new NotImplementedException();
        }

        public Task<Client> EditingClient(CreateClientModel model)
        {
            return _clientRepository.EditingClient(model.Company, model.Client, model.User);
        }

        public Task<Client> GetInfoClient(ClientDto client)
        {
            return _clientRepository.GetInfoClient(client);
        }

        public Task<string> GetInfoListClients(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoSalesClient(string data)
        {
            throw new NotImplementedException();
        }
    }
}
