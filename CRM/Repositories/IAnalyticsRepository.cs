using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IAnalyticsRepository
    {
        Task<AnalyticsDto> GetAnalytics(CompanyDto company, DateTime fromDate, DateTime toDate);
    }
}
