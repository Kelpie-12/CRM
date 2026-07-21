using System.Data.SqlClient;
using CRM.Model.DTO;

namespace CRM.Repositories.Implementations
{
    public class SettingsRepository : BaseRepository, ISettingsRepository
    {
        public SettingsRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<CompanySettingsDto> GetSettings(int companyId, string companyName)
        {
            var settings = new CompanySettingsDto
            {
                CompanyId = companyId,
                CompanyName = companyName,
                Description = string.Empty,
                CashboxId = 0,
                CashboxName = string.Empty,
                Cashboxes = new List<CashboxDto>()
            };

            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT TOP 1 ISNULL([Description], ''), ISNULL([Name], @CompanyName) " +
                        "FROM [Enterprise]";
                    cmd.Parameters.AddWithValue("CompanyName", companyName);

                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows && reader.Read())
                        {
                            settings.Description = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            settings.CompanyName = reader.IsDBNull(1) ? companyName : reader.GetString(1);
                        }
                        reader.Close();
                    }
                    catch
                    {
                        // Enterprise table may not exist yet in some company databases.
                    }
                }

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cashbox ORDER BY Id";
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            settings.Cashboxes.Add(new CashboxDto
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                        reader.Close();
                    }
                    catch
                    {
                        settings.Cashboxes.Add(new CashboxDto { Id = 1, Name = "Main" });
                    }
                }

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT TOP 1 CAST([Value] AS INT) " +
                        "FROM [Settings] WHERE [Key]='ConnectedCashboxId'";
                    try
                    {
                        object? result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            settings.CashboxId = Convert.ToInt32(result);
                        }
                    }
                    catch
                    {
                        // Settings table may not exist yet.
                    }
                }

                con.Close();
            }

            if (settings.CashboxId == 0 && settings.Cashboxes.Count > 0)
            {
                settings.CashboxId = 1;// settings.Cashboxes[0].Id;
            }

            settings.CashboxName = settings.Cashboxes
                .FirstOrDefault(c => c.Id == settings.CashboxId)?.Name
                ?? settings.Cashboxes.FirstOrDefault()?.Name
                ?? "Main";

            return settings;
        }

        public async Task<bool> UpdateCompanyName(int companyId, string companyName, string newName)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE [Enterprise] SET [Name]=@NewName";
                    cmd.Parameters.AddWithValue("NewName", newName);
                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch
                    {
                        // Ignore if Enterprise table is missing.
                    }
                }
                con.Close();
            }

            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "USE [CRM] UPDATE [Company] SET [Name]=@NewName WHERE [Id]=@CompanyId";
                    cmd.Parameters.AddWithValue("NewName", newName);
                    cmd.Parameters.AddWithValue("CompanyId", companyId);
                    await cmd.ExecuteNonQueryAsync();
                }
                con.Close();
            }

            return true;
        }

        public async Task<bool> UpdateCompanyDescription(string companyName, string description)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "IF EXISTS (SELECT 1 FROM [Enterprise]) " +
                        "UPDATE [Enterprise] SET [Description]=@Description " +
                        "ELSE INSERT INTO [Enterprise] ([Name],[Description]) VALUES (@CompanyName,@Description)";
                    cmd.Parameters.AddWithValue("Description", description ?? string.Empty);
                    cmd.Parameters.AddWithValue("CompanyName", companyName);
                    await cmd.ExecuteNonQueryAsync();
                }
                con.Close();
            }

            return true;
        }

        public async Task<bool> UpdateConnectedCashbox(string companyName, int cashboxId)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "IF EXISTS (SELECT 1 FROM [Settings] WHERE [Key]='ConnectedCashboxId') " +
                        "UPDATE [Settings] SET [Value]=@CashboxId WHERE [Key]='ConnectedCashboxId' " +
                        "ELSE INSERT INTO [Settings] ([Key],[Value]) VALUES ('ConnectedCashboxId', @CashboxId)";
                    cmd.Parameters.AddWithValue("CashboxId", cashboxId);
                    await cmd.ExecuteNonQueryAsync();
                }
                con.Close();
            }

            return true;
        }
    }
}
