namespace CRM.Model.DTO
{
    public class AnalyticsDto
    {
        public decimal CashBalance { get; set; }
        public string CashboxName { get; set; } = string.Empty;
        public decimal TotalProductsCost { get; set; }
        public decimal TotalSalesRevenue { get; set; }
        public int TotalSalesCount { get; set; }
        public List<SalesChartPointDto> SalesChart { get; set; } = new();
        public List<EmployeeSalesMetricDto> EmployeeMetrics { get; set; } = new();
    }

    public class SalesChartPointDto
    {
        public DateTime Date { get; set; }
        public int SalesCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class EmployeeSalesMetricDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
        public int ItemsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
