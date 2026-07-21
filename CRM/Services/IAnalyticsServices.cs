using CRM.Model.DTO;

namespace CRM.Services
{
    public interface IAnalyticsServices
    {
        Task<AnalyticsDto> GetAnalytics(CompanyDto company, DateTime? fromDate, DateTime? toDate);
    }
}
