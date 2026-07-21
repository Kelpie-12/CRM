using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface ISettingsRepository
    {
        Task<CompanySettingsDto> GetSettings(int companyId, string companyName);
        Task<bool> UpdateCompanyName(int companyId, string companyName, string newName);
        Task<bool> UpdateCompanyDescription(string companyName, string description);
        Task<bool> UpdateConnectedCashbox(string companyName, int cashboxId);
    }
}
