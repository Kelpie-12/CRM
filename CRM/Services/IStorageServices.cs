using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface IStorageServices
    {
        Task<CreateItemResponse> CreateItem(ItemModel item);
        Task<bool> WriteOffItem(string data);
        Task<bool> ArrivalItem(string data);
        Task<Item> UpdateItem(ItemModel item);
        Task<bool> DeleteItem(ItemModel item);
        Task<ICollection<Item>> GetAll(CompanyDto company);
        Task<string> GetInfoItem(string data);
        Task<string> GetInfoWriteOffItem(string data);
        Task<string> GetInfoArrivalItem(string data);
        Task<string> GetInfoGroupItem(string data);

    }
}
