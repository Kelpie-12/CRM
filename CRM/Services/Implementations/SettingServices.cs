using CRM.Model.Data;
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
    }
}
