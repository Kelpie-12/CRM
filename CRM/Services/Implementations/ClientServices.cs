
using System.Text.Json;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
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
        public Task<ClientResponse> CreateClient(ClientModel model)
        {
            return _clientRepository.CreateClient(model.Company, model.Client, model.User);
        }

        public Task<bool> CreateSalesForClient(string data)
        {
            throw new NotImplementedException();
        }

        public Task<ClientResponse> UpdateClient(ClientModel model)
        {
            return _clientRepository.UpdateClient(model.Company, model.Client, model.User);
        }

        public async Task<Client> GetInfoClient(InfoClientModel model)
        {

            Client res = await _clientRepository.GetInfoClient(model);

            return res;
        }

        public async Task<ICollection<Client>> GetInfoListClients(int companyId, int status)
        {
            return await _clientRepository.GetInfoListClients(companyId, status);
        }

        public Task<string> GetInfoSalesClient(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<Task> DeleteClient(ClientModel model)
        {
            if (model == null)
            {
                //дописать различное поведение
            }
            bool res =  _clientRepository.DeleteClient(model.Company, model.Client, model.User).IsCompleted;
            if (res)
            {
                return Task.CompletedTask;
            }else
                throw new ArgumentNullException("");
        }
    }
}
