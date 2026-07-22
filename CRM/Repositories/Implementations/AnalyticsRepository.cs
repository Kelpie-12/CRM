using System.Data.SqlClient;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;

namespace CRM.Repositories.Implementations
{
    public class AnalyticsRepository : BaseRepository, IAnalyticsRepository
    {
        public AnalyticsRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<AnalyticsDto> GetAnalytics(CompanyDto company, DateTime fromDate, DateTime toDate)
        {
            AnalyticsDto analytics = new AnalyticsDto();
            DateTime toDateExclusive = toDate.Date.AddDays(1);

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();

                int cashboxId = await GetConnectedCashboxId(con);
                await LoadCashBalance(con, analytics, cashboxId);
                await LoadTotalProductsCost(con, analytics);
                await LoadSalesChart(con, analytics, fromDate, toDateExclusive);
                await LoadEmployeeMetrics(con, analytics, fromDate, toDateExclusive);

                con.Close();
            }

            FillSalesChartGaps(analytics, fromDate.Date, toDate.Date);
            analytics.TotalSalesCount = analytics.SalesChart.Sum(point => point.SalesCount);
            analytics.TotalSalesRevenue = analytics.SalesChart.Sum(point => point.Revenue);

            return analytics;
        }

        private static void FillSalesChartGaps(AnalyticsDto analytics, DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, SalesChartPointDto> pointsByDate = analytics.SalesChart
                .ToDictionary(point => point.Date.Date);

            List<SalesChartPointDto> filledChart = new List<SalesChartPointDto>();

            for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                if (pointsByDate.TryGetValue(date, out SalesChartPointDto? existingPoint))
                {
                    filledChart.Add(existingPoint);
                    continue;
                }

                filledChart.Add(new SalesChartPointDto
                {
                    Date = date,
                    SalesCount = 0,
                    Revenue = 0,
                });
            }

            analytics.SalesChart = filledChart;
        }

        private static async Task<int> GetConnectedCashboxId(SqlConnection con)
        {
            using SqlCommand cmd = con.CreateCommand();
            cmd.CommandText =
                "SELECT TOP 1 CAST([Value] AS INT) FROM [Settings] WHERE [Key]='ConnectedCashboxId'";

            try
            {
                object? result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                // Settings table may not exist yet.
            }

            return 1;
        }

        private static async Task LoadCashBalance(SqlConnection con, AnalyticsDto analytics, int cashboxId)
        {
            using SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 Id, Name, Balance FROM Cashbox WHERE Id=@CashboxId";
            cmd.Parameters.AddWithValue("CashboxId", cashboxId);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                analytics.CashboxName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                analytics.CashBalance = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
            }
            else
            {
                reader.Close();

                using SqlCommand fallbackCmd = con.CreateCommand();
                fallbackCmd.CommandText = "SELECT TOP 1 Id, Name, Balance FROM Cashbox ORDER BY Id";
                reader = await fallbackCmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    analytics.CashboxName = reader.IsDBNull(1) ? "Main" : reader.GetString(1);
                    analytics.CashBalance = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                }
            }

            reader.Close();
        }

        private static async Task LoadTotalProductsCost(SqlConnection con, AnalyticsDto analytics)
        {
            using SqlCommand cmd = con.CreateCommand();
            cmd.CommandText =
                "SELECT ISNULL(SUM([Storage].PurchasePrice * [Storage].[Count]), 0) " +
                "FROM [Storage] WHERE [Storage].Status=@Status";
            cmd.Parameters.AddWithValue("Status", Status.Active);

            object? result = await cmd.ExecuteScalarAsync();
            analytics.TotalProductsCost = result == null || result == DBNull.Value
                ? 0
                : Convert.ToDecimal(result);
        }

        private static async Task LoadSalesChart(
            SqlConnection con,
            AnalyticsDto analytics,
            DateTime fromDate,
            DateTime toDateExclusive)
        {
            using SqlCommand cmd = con.CreateCommand();
            cmd.CommandText =
                "SELECT CAST(Sales.CreatedAt AS DATE) AS SaleDate, " +
                "COUNT(Sales.Id) AS SalesCount, " +
                "ISNULL(SUM(Sales.[Count] * [Storage].Price), 0) AS Revenue " +
                "FROM Sales " +
                "INNER JOIN [Storage] ON [Storage].Id = Sales.ItemId " +
                "WHERE Sales.Status=@Status " +
                "AND Sales.CreatedAt >= @FromDate AND Sales.CreatedAt < @ToDate " +
                "GROUP BY CAST(Sales.CreatedAt AS DATE) " +
                "ORDER BY SaleDate";
            cmd.Parameters.AddWithValue("Status", Status.Active);
            cmd.Parameters.AddWithValue("FromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("ToDate", toDateExclusive);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                analytics.SalesChart.Add(new SalesChartPointDto
                {
                    Date = reader.GetDateTime(0),
                    SalesCount = reader.GetInt32(1),
                    Revenue = reader.GetDecimal(2),
                });
            }

            reader.Close();
        }

        private static async Task LoadEmployeeMetrics(
            SqlConnection con,
            AnalyticsDto analytics,
            DateTime fromDate,
            DateTime toDateExclusive)
        {
            using SqlCommand cmd = con.CreateCommand();
            cmd.CommandText =
                "SELECT Employee.Id, Employee.FirstName, Employee.LastName, " +
                "COUNT(Sales.Id) AS SalesCount, " +
                "ISNULL(SUM(Sales.[Count]), 0) AS ItemsSold, " +
                "ISNULL(SUM(Sales.[Count] * [Storage].Price), 0) AS Revenue " +
                "FROM Sales " +
                "INNER JOIN Employee ON Employee.Id = Sales.EmployeeId " +
                "INNER JOIN [Storage] ON [Storage].Id = Sales.ItemId " +
                "WHERE Sales.Status=@Status " +
                "AND Sales.CreatedAt >= @FromDate AND Sales.CreatedAt < @ToDate " +
                "GROUP BY Employee.Id, Employee.FirstName, Employee.LastName " +
                "ORDER BY Revenue DESC";
            cmd.Parameters.AddWithValue("Status", Status.Active);
            cmd.Parameters.AddWithValue("FromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("ToDate", toDateExclusive);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                analytics.EmployeeMetrics.Add(new EmployeeSalesMetricDto
                {
                    EmployeeId = reader.GetInt32(0),
                    FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    SalesCount = reader.GetInt32(3),
                    ItemsSold = reader.GetInt32(4),
                    Revenue = reader.GetDecimal(5),
                });
            }

            reader.Close();
        }
    }
}
