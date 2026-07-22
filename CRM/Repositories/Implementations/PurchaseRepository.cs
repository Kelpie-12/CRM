using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;

namespace CRM.Repositories.Implementations
{
    public class PurchaseRepository : BaseRepository, IPurchaseRepository
    {
        public PurchaseRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<PurchaseResponse> CreatePurchase(PurchaseModel model)
        {
            PurchaseResponse response = new PurchaseResponse
            {
                Message = "Purchase created!",
                Error = "none",
                Purchase = new Purchase()
            };

            using (SqlConnection con = CreateConnection(model.Company!.Name))
            {
                con.Open();              

                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandText =
                    "BEGIN TRANSACTION; " +
                    "DECLARE @EmployeeId INT; " +
                    "DECLARE @Article INT; " +
                    "DECLARE @PurchasePrice DECIMAL(10,2); " +
                    "DECLARE @StatusId INT; " +
                    "DECLARE @FiscalDocumentId INT; " +
                    "DECLARE @TotalAmount DECIMAL(10,2); " +

                    "SET @EmployeeId = (SELECT TOP 1 Employee.Id FROM Employee WHERE Employee.UserId=@UserId); " +
                    "SET @Article = (SELECT TOP 1 Storage.Article FROM Storage WHERE Storage.Id=@ItemId); " +

                    "SET @PurchasePrice = CASE WHEN @ProvidedPrice > 0 THEN @ProvidedPrice " +
                    "ELSE (SELECT TOP 1 Storage.PurchasePrice FROM Storage WHERE Storage.Id=@ItemId) END; " +

                    "SET @StatusId = (SELECT TOP 1 [Status].Id FROM [Status] WHERE [Status].[Name]='Active'); " +
                    "SET @TotalAmount = @Count * @PurchasePrice; " +

                    "IF @EmployeeId IS NULL OR @Article IS NULL BEGIN " +
                    "SELECT -1 AS Id; " +
                    "END ELSE BEGIN " +

                    "INSERT INTO FiscalDocumentNumber (ItemId, [Count], OperationType, Amount, Status) " +
                    "VALUES (@ItemId, @Count, " +
                    "(SELECT TOP 1 OperationType.Id FROM OperationType WHERE OperationType.Name='Outcome'), " +
                    "@TotalAmount, @StatusId); " +
                    "SET @FiscalDocumentId = SCOPE_IDENTITY(); " +

                    "INSERT INTO [Purchase] (EmployeeId, ItemId, ItemArticle, [Count], FiscalDocumentNumber, PurchasePrice, Status) " +
                    "VALUES (@EmployeeId, @ItemId, @Article, @Count, @FiscalDocumentId, @PurchasePrice, @StatusId); " +
                    //???????? amount ?? Amount
                    "insert into Transactions (CashboxId,OperationType,amount,FiscalDocumentNumber,[Status]) " +
                    "values ((select Id from Cashbox where Cashbox.Name='Main')," +
                    "        (select Id from OperationType where OperationType.Name='Outcome')," +
                    "        @TotalAmount," +
                    "        @FiscalDocumentId," +
                    "        @ActiveStatus);" +

                    "declare @balanceNow as int " +
                    "set @balanceNow = (select Cashbox.Balance from Cashbox " +
                    "where Cashbox.Id =(select Id from Cashbox where Cashbox.[Name]=@CashboxName))" +

                    "update Cashbox set Cashbox.Balance=@balanceNow -(@Count*(select Price from Storage where Storage.Id=@ItemId))" +
                    "where Cashbox.Id =(select Id from Cashbox where Cashbox.[Name]=@CashboxName)" +

                    "UPDATE Storage SET Storage.[Count] = Storage.[Count] + @Count WHERE Storage.Id=@ItemId; " +
                    "SELECT Purchase.Id, Purchase.EmployeeId, Purchase.ItemArticle, Purchase.[Count], " +
                    "Purchase.CreatedAt, Purchase.Status, Purchase.ItemId, Purchase.PurchasePrice, Purchase.FiscalDocumentNumber " +
                    "FROM Purchase WHERE Purchase.Id = SCOPE_IDENTITY(); " +
                    "END " +
                    "COMMIT;";

                cmd.Parameters.AddWithValue("UserId", model.Employee!.Id);
                cmd.Parameters.AddWithValue("ItemId", model.Item!.Id);
                cmd.Parameters.AddWithValue("Count", model.Item.Count);
                cmd.Parameters.AddWithValue("CashboxName","Main");//????????
                cmd.Parameters.AddWithValue("ProvidedPrice", model.Item.PurchasePrice ?? 0);
                cmd.Parameters.AddWithValue("ActiveStatus", Status.Active);        

                try
                {
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        int purchaseId = reader.GetInt32(0);
                        if (purchaseId == -1)
                        {
                            response.Purchase.Id = -1;
                            response.Message = "Unable to create purchase. Check product and employee.";
                            response.Error = "error_create_purchase";
                        }
                        else
                        {
                            MapPurchase(reader, response.Purchase);
                        }
                    }
                }
                catch (Exception e)
                {
                    response.Message = e.Message;
                    response.Error = "error_create_purchase";
                    response.Purchase.Id = -1;
                }

                con.Close();
            }

            return response;
        }

        public async Task<Task> DeletePurchase(int purchaseId, CompanyDto company)
        {
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
        

                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandText =
            "BEGIN TRANSACTION; " +
            "Update Storage " +
            "set [Count]+=(select Purchase.[Count] " +
            "from Purchase where Purchase.Id=@PurchaseId) " +
            "where Id=(select Purchase.ItemId from Purchase where Id=@PurchaseId); " +

            "update Cashbox " +
            "set Cashbox.Balance+=((select Purchase.[Count] from Purchase where Purchase.Id=@PurchaseId) " +
            "                      *(select  Purchase.PurchasePrice from Purchase where Purchase.Id=@PurchaseId)) " +
            "where Cashbox.Id =(select Id from Cashbox where Cashbox.[Name]='Main') " +

            "update Transactions " +
            "set [Status]=(SELECT TOP 1 [Status].Id FROM [Status] WHERE [Status].[Name]='Canceled') " +
            "where Transactions.FiscalDocumentNumber=(SELECT TOP 1 Purchase.FiscalDocumentNumber FROM Purchase WHERE Purchase.Id=@PurchaseId) " +
            "update FiscalDocumentNumber " +
            "set [Status]=(SELECT TOP 1 [Status].Id FROM [Status] WHERE [Status].[Name]='Canceled') " +
            "where FiscalDocumentNumber.Id=(SELECT TOP 1 Purchase.FiscalDocumentNumber FROM Purchase WHERE Purchase.Id=@PurchaseId) " +
           
            "update Purchase " +
            "set [Status]=(SELECT TOP 1 [Status].Id FROM [Status] WHERE [Status].[Name]='Canceled')" +
            "where Id=@PurchaseId " +
            "commit;";
                cmd.Parameters.AddWithValue("PurchaseId", purchaseId);          

              
                await cmd.ExecuteNonQueryAsync();
                con.Close();
            }

            return Task.CompletedTask;
        }

        public async Task<ICollection<Purchase>> GetAll(CompanyDto company, int status)
        {
            List<Purchase> purchases = new List<Purchase>();

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
              

                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandText =
                    "SELECT Purchase.Id, Purchase.EmployeeId, Purchase.ItemArticle, Purchase.[Count], " +
                    "Purchase.CreatedAt, Purchase.Status, Purchase.ItemId, Purchase.PurchasePrice, Purchase.FiscalDocumentNumber " +
                    "FROM Purchase WHERE Purchase.Status=@Status ORDER BY Purchase.Id DESC";
                cmd.Parameters.AddWithValue("Status", status);

                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Purchase purchase = new Purchase();
                    MapPurchase(reader, purchase);
                    purchases.Add(purchase);
                }

                con.Close();
            }

            return purchases;
        }

        private static void MapPurchase(SqlDataReader reader, Purchase purchase)
        {
            purchase.Id = reader.GetInt32(0);
            purchase.EmployeeId = reader.GetInt32(1);
            purchase.ItemArticle = reader.GetInt32(2);
            purchase.Count = reader.GetInt32(3);
            purchase.CreatedAt = reader.GetDateTime(4);
            purchase.Status = reader.GetInt32(5);
            purchase.ItemId = reader.GetInt32(6);
            purchase.PurchasePrice = reader.GetDecimal(7);
            purchase.FiscalDocumentNumber = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
        }

        
        
    }
}
