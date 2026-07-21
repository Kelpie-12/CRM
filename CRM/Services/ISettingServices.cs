using CRM.Model.Data;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface ISettingServices
    {
        Task<CompanySettingsDto> GetSettings(int companyId, string companyName);
        Task<bool> UpdateCompanyName(UpdateCompanySettingsModel model);
        Task<bool> UpdateCompanyDescription(UpdateCompanySettingsModel model);
        Task<bool> UpdateConnectedCashbox(UpdateCompanySettingsModel model);
    }
}
