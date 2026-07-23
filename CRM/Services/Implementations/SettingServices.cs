using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class SettingServices : ISettingServices
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingServices(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public Task<CompanySettingsDto> GetSettings(int companyId, string companyName)
        {
            return _settingsRepository.GetSettings(companyId, companyName);
        }

        public Task<bool> UpdateCompanyName(UpdateCompanySettingsModel model)
        {
            return _settingsRepository.UpdateCompanyName(
                model.Company.Id??-1,
                model.Company.Name,
                model.CompanyName ?? model.Company.Name);
        }

        public Task<bool> UpdateCompanyDescription(UpdateCompanySettingsModel model)
        {
            return _settingsRepository.UpdateCompanyDescription(
                model.Company.Name,
                model.Description ?? string.Empty);
        }

        public Task<bool> UpdateConnectedCashbox(UpdateCompanySettingsModel model)
        {
            return _settingsRepository.UpdateConnectedCashbox(
                model.Company.Name,
                model.CashboxId ?? 0);
        }

        public Task<CompanyRoleResponse> CreateRole(CompanyRoleModel model)
        {
            return CreateRoleInternal(model);
        }

        public Task<CompanyRoleResponse> UpdateRole(CompanyRoleModel model)
        {
            return UpdateRoleInternal(model);
        }

        public Task<CompanyRoleResponse> DeleteRole(CompanyRoleModel model)
        {
            return DeleteRoleInternal(model);
        }

        private async Task<CompanyRoleResponse> CreateRoleInternal(CompanyRoleModel model)
        {
            if (string.IsNullOrWhiteSpace(model.RoleName))
            {
                return FailRole("Position name is required.", "error_role_name_required");
            }

            CompanyRoleDto? role = await _settingsRepository.CreateRole(
                model.Company.Name,
                model.RoleName.Trim(),
                model.Permissions ?? new List<string>());

            if (role == null)
            {
                return FailRole("Unable to create position. It may already exist.", "error_create_role");
            }

            return new CompanyRoleResponse
            {
                Message = "Position created.",
                Error = "none",
                Role = role
            };
        }

        private async Task<CompanyRoleResponse> UpdateRoleInternal(CompanyRoleModel model)
        {
            if (!model.RoleId.HasValue || model.RoleId <= 0)
            {
                return FailRole("Position id is required.", "error_role_id_required");
            }

            CompanyRoleDto? role = await _settingsRepository.UpdateRole(
                model.Company.Name,
                model.RoleId.Value,
                model.RoleName,
                model.Permissions ?? new List<string>());

            if (role == null)
            {
                return FailRole("Unable to update position.", "error_update_role");
            }

            return new CompanyRoleResponse
            {
                Message = "Position updated.",
                Error = "none",
                Role = role
            };
        }

        private async Task<CompanyRoleResponse> DeleteRoleInternal(CompanyRoleModel model)
        {
            if (!model.RoleId.HasValue || model.RoleId <= 0)
            {
                return FailRole("Position id is required.", "error_role_id_required");
            }

            bool deleted = await _settingsRepository.DeleteRole(model.Company.Name, model.RoleId.Value);
            if (!deleted)
            {
                return FailRole(
                    "Unable to delete position. System positions and positions in use cannot be removed.",
                    "error_delete_role");
            }

            return new CompanyRoleResponse
            {
                Message = "Position deleted.",
                Error = "none"
            };
        }

        private static CompanyRoleResponse FailRole(string message, string error)
        {
            return new CompanyRoleResponse
            {
                Message = message,
                Error = error,
                Role = null
            };
        }
    }
}
