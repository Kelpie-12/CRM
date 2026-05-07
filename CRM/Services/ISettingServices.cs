namespace CRM.Services
{
    public interface ISettingServices
    {
        Task<bool> CreateEmployee(string data);
        Task<bool> EditingEmployee(string data);
        Task<bool> DeleteEmployee(string data);
        Task<bool> RecoveryEmployee(string data);
        Task<bool> EditingInfoAboutEnterprise(string data);
        Task<bool> EditingSettings(string data);
        Task<string> GetInfoListSales(string data);
        Task<string> GetInfoSetting(string data);
        Task<string> GetInfoAboutEnterprise(string data);
    }
}
