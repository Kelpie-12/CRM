using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Repositories.Implementations
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        public TransactionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<ICollection<Transaction>> GetAll(CompanyDto company, int status)
        {
            List<Transaction> transactions = new List<Transaction>();

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT t.Id, cb.Name, ot.Name, t.Amount, t.FiscalDocumentNumber, " +
                        "t.CreatedAt, t.Status, st.Name " +
                        "FROM Transactions t " +
                        "INNER JOIN Cashbox cb ON cb.Id = t.CashboxId " +
                        "INNER JOIN OperationType ot ON ot.Id = t.OperationType " +
                        "INNER JOIN [Status] st ON st.Id = t.Status " +
                        "LEFT JOIN Sales s ON s.TransactionsId = t.Id " +
                        "WHERE t.Status = @Status " +
                        "ORDER BY t.Id DESC";
                    cmd.Parameters.AddWithValue("Status", status);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        transactions.Add(new Transaction
                        {
                            Id = reader.GetInt32(0),
                            CashboxName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            OperationType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Amount = reader.GetDecimal(3),
                            FiscalDocumentNumber = reader.GetInt32(4),
                            CreatedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                            Status = reader.GetInt32(6),
                            StatusName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                        });
                    }
                }
                con.Close();
            }

            return transactions;
        }

        public async Task<FiscalDocumentDto?> GetFiscalDocument(CompanyDto company, int fiscalDocumentId)
        {
            FiscalDocumentDto? document = null;

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT TOP 1 fd.Id, fd.ItemId, fd.Count, fd.Amount, ot.Name, st.Name, " +
                        "storage.Name, storage.Article, cb.Name, t.Amount, fd.CreatedAt " +
                        "FROM FiscalDocumentNumber fd " +
                        "LEFT JOIN OperationType ot ON ot.Id = fd.OperationType " +
                        "LEFT JOIN [Status] st ON st.Id = fd.Status " +
                        "LEFT JOIN Storage storage ON storage.Id = fd.ItemId " +
                        "LEFT JOIN Transactions t ON t.FiscalDocumentNumber = fd.Id " +
                        "LEFT JOIN Cashbox cb ON cb.Id = t.CashboxId " +
                        "LEFT JOIN Sales s ON s.TransactionsId = t.Id " +
                        "WHERE fd.Id = @FiscalDocumentId";
                    cmd.Parameters.AddWithValue("FiscalDocumentId", fiscalDocumentId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        document = new FiscalDocumentDto
                        {
                            Id = reader.GetInt32(0),
                            ItemId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            Count = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                            Amount = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                            OperationType = reader.IsDBNull(4) ? null : reader.GetString(4),
                            StatusName = reader.IsDBNull(5) ? null : reader.GetString(5),
                            ProductName = reader.IsDBNull(6) ? null : reader.GetString(6),
                            ProductArticle = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                            CashboxName = reader.IsDBNull(8) ? null : reader.GetString(8),
                            TransactionAmount = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                            CreatedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                        };
                    }
                }
                con.Close();
            }

            return document;
        }
    }
}
