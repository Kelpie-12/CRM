namespace CRM.Services
{
    public interface IStorageServices
    {
        Task<bool> CreateItem(string data);
        Task<bool> WriteOffItem(string data);
        Task<bool> ArrivalItem(string data);
        Task<bool> EditingItem(string data);
        Task<string> GetInfoItem(string data);
        Task<string> GetInfoWriteOffItem(string data);
        Task<string> GetInfoArrivalItem(string data);
        Task<string> GetInfoGroupItem(string data);

    }
}
