
namespace CRM.Repositories.Implementations
{
    public class ClientRepository :BaseRepository, IClientRepository
    {
        public ClientRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public Task<bool> CreateClient(string data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateSalesForClient(string data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditingClient(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoClient(string data)
        {
            throw new NotImplementedException();
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
