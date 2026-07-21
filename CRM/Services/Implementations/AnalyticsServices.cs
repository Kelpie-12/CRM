using CRM.Model.DTO;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class AnalyticsServices : IAnalyticsServices
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsServices(IAnalyticsRepository repository)
        {
            _analyticsRepository = repository;
        }

        public Task<AnalyticsDto> GetAnalytics(CompanyDto company, DateTime? fromDate, DateTime? toDate)
        {
            DateTime to = toDate?.Date ?? DateTime.UtcNow.Date;
            DateTime from = fromDate?.Date ?? to.AddDays(-29);

            if (from > to)
            {
                (from, to) = (to, from);
            }

            return _analyticsRepository.GetAnalytics(company, from, to);
        }
    }
}
