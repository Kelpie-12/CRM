using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface ISettingsRepository
    {
        Task<CompanySettingsDto> GetSettings(int companyId, string companyName);
        Task<bool> UpdateCompanyName(int companyId, string companyName, string newName);
        Task<bool> UpdateCompanyDescription(string companyName, string description);
        Task<bool> UpdateConnectedCashbox(string companyName, int cashboxId);
        Task<CompanyRoleDto?> CreateRole(string companyName, string roleName, ICollection<string> permissions);
        Task<CompanyRoleDto?> UpdateRole(string companyName, int roleId, string? roleName, ICollection<string> permissions);
        Task<bool> DeleteRole(string companyName, int roleId);
    }
}
