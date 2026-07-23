using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface ISettingServices
    {
        Task<CompanySettingsDto> GetSettings(int companyId, string companyName);
        Task<bool> UpdateCompanyName(UpdateCompanySettingsModel model);
        Task<bool> UpdateCompanyDescription(UpdateCompanySettingsModel model);
        Task<bool> UpdateConnectedCashbox(UpdateCompanySettingsModel model);
        Task<CompanyRoleResponse> CreateRole(CompanyRoleModel model);
        Task<CompanyRoleResponse> UpdateRole(CompanyRoleModel model);
        Task<CompanyRoleResponse> DeleteRole(CompanyRoleModel model);
    }
}
