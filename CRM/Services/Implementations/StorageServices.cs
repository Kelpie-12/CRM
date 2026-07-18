
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class StorageServices : IStorageServices
    {
        private readonly IStorageRepository _storageRepository;

        public StorageServices(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }
        public Task<bool> ArrivalItem(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemResponse> CreateItem(ItemModel item)
        {
            //проверки
            return await _storageRepository.CreateItem(item);
            
        }
        public async Task<ItemResponse> UpdateItem(ItemModel item)
        {
            return await _storageRepository.UpdateItem(item);
        }
        public async Task<bool> DeleteItem(ItemModel item)
        {
            if (await _storageRepository.DeleteItem(item)==Task.CompletedTask)
            {
                return true;
            }
           return false;
        }

        public async Task<ICollection<Item>> GetAll(CompanyDto company, int status)
        {
            return await _storageRepository.GetAll(company, status);
        }

        public Task<string> GetInfoArrivalItem(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoGroupItem(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoItem(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoWriteOffItem(string data)
        {
            throw new NotImplementedException();
        }

      

        public Task<bool> WriteOffItem(string data)
        {
            throw new NotImplementedException();
        }
    }
}
