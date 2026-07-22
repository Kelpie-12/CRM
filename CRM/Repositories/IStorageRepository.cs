using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IStorageRepository
    {
        Task<ItemResponse> CreateItem(ItemModel item);
        Task<bool> WriteOffItem(string data);
        Task<bool> ArrivalItem(string data);
        Task<Task> DeleteItem(ItemModel item);
        Task<ItemResponse> UpdateItem(ItemModel item);
        Task<ICollection<Item>> GetAll(CompanyDto company, int status, string? statusName = null);
        Task<string> GetInfoItem(string data);
        Task<string> GetInfoWriteOffItem(string data);
        Task<string> GetInfoArrivalItem(string data);
        Task<string> GetInfoGroupItem(string data);
    }
}
