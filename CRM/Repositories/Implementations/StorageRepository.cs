
using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;

namespace CRM.Repositories.Implementations
{
    public class StorageRepository : BaseRepository, IStorageRepository
    {
        public StorageRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Task<bool> ArrivalItem(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemResponse> CreateItem(ItemModel item)
        {
            ItemResponse response = new ItemResponse()
            {
                Message = "Item was created!",
                Error = "none",
                Item = new Item()
            };
            using (SqlConnection con = CreateConnection(item.Company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM [Storage] WHERE [Storage].Article=@Article) " +
                                      "BEGIN " +
                                      "declare @e as int " +
                                      "set @e =(select [Employee].[Id] from [Employee] where [Employee].[UserId]=@UserId)" +
                                      "INSERT INTO [Storage] ([Storage].Name,[Storage].Article,[Storage].[Status],[Storage].EmployeeCreateId, " +
                                      "[Storage].Count,[Storage].Price,[Storage].PurchasePrice) " +
                                      "VALUES (@Name,@Article,@Status,@e,@Count,@Price,@PurchasePrice); " +
                                      "select [Storage].[Name],[Storage].[Id],[Storage].Article," +
                                      "[Storage].[Count],[Storage].Price,[Storage].PurchasePrice,[Storage].[Status] " +
                                      "from [Storage] where [Storage].Article=@Article " +
                                      "END " +
                                      "ELSE BEGIN " +
                                      "SELECT 'Item already exist' AS Result; " +
                                      "END ";
                    cmd.Parameters.AddWithValue("Article", item.Item.Article);
                    cmd.Parameters.AddWithValue("Name", item.Item.Name);
                    cmd.Parameters.AddWithValue("UserId", item.User.Id);
                    cmd.Parameters.AddWithValue("Count", item.Item.Count);
                    cmd.Parameters.AddWithValue("Price", item.Item.Price);
                    cmd.Parameters.AddWithValue("PurchasePrice", item.Item.PurchasePrice ?? 1);

                    cmd.Parameters.AddWithValue("Status", Status.Active);


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader.GetString(0) != "Item already exist")
                                {
                                    response.Item.Name = reader.GetString(0);
                                    response.Item.Id = reader.GetInt32(1);
                                    response.Item.Article = reader.GetInt32(2);
                                    response.Item.Count = reader.GetInt32(3);
                                    response.Item.Price = reader.GetDecimal(4);
                                    response.Item.PurchasePrice = reader.GetDecimal(5);
                                    response.Item.Status = reader.GetInt32(6);
                                }
                                else
                                {
                                    response.Item.Id = -1;
                                    response.Message = "Item already exist";
                                    response.Error = "error_item_already_exist";
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Error = e.Message;
                        response.Message = "Created item was fail! Check data!";
                        response.Item.Id = -1;
                    }

                }

               

                con.Close();
            }


            return response;
        }

        public async Task<Task> DeleteItem(ItemModel item)
        {
            using (SqlConnection con = CreateConnection(item.Company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "declare @eId as int " +
                                      "set @eId=(select [Employee].[Id] from [Employee] where [Employee].[UserId]=@UserId)" +
                                      "update [Storage] " +
                                      "set [Status]=@Status,[EmloyeeUpdateId]=@eId " +
                                      "where [Storage].Id=@StorageId ";
                    cmd.Parameters.AddWithValue("StorageId", item.Item.Id);
                    cmd.Parameters.AddWithValue("UserId", item.User.Id);
                    cmd.Parameters.AddWithValue("Status", Status.Deleted);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 

                }
                con.Close();
            }
            return Task.CompletedTask;
            //  return Task.FromResult(false);
        }

        public async Task<ICollection<Item>> GetAll(CompanyDto company, int status, string? statusName = null)
        {

            List<Item> res = new List<Item>();
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();           

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select  [Storage].Article,[Storage].Name," +
                                      "[Storage].Count,[Storage].Price,[Storage].[Status],[Storage].Id," +
                                      "[Storage].PurchasePrice,[Storage].CreatedAt,[Status].[Name] as StatusName " +
                                      "from [Storage] " +
                                      "inner join [Status] on [Status].Id=[Storage].[Status] " +
                                      "where [Storage].Status=" +
                                      (string.IsNullOrWhiteSpace(statusName)
                                          ? "@Status"
                                          : "(SELECT TOP 1 Id FROM [Status] WHERE [Name]=@StatusName)");

                    if (string.IsNullOrWhiteSpace(statusName))
                    {
                        cmd.Parameters.AddWithValue("Status", status);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("StatusName", statusName);
                    }
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Item item = new Item();
                            item.Article = reader.GetInt32(0);
                            item.Name = reader.GetString(1);
                            item.Count = reader.GetInt32(2);
                            item.Price = reader.GetDecimal(3);
                            //item.Price = reader.GetInt32(3);
                            item.Status = reader.GetInt32(4);
                            item.Id = reader.GetInt32(5);
                            item.PurchasePrice = reader.GetDecimal(6);
                            item.CreatedAt = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7);
                            item.StatusName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);
                            res.Add(item);
                        }
                    }
                }
                con.Close();
            }


            return await Task.FromResult(res);
        }

        public Task<string> GetInfoArrivalItem(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoGroupItem(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoItem(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoWriteOffItem(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemResponse> UpdateItem(ItemModel item)
        {
            ItemResponse res = new ItemResponse()
            {
                Error = "none",
                Message = "Item was updated!",
                Item = new Item()
            };
            using (SqlConnection con = CreateConnection(item.Company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "declare @eId as int " +
                                      "set @eId=(select [Employee].[Id] from [Employee] where [Employee].[UserId]=@UserId)" +
                                      "update [Storage] " +
                                      "set [Name]=@Name,Article=@Article,[Count]=@Count,Price=@Price," +
                                      "PurchasePrice=@PurchasePrice,EmployeeUpdateId=@eId " +
                                      "where Id=@Id " +
                                      "select [Storage].[Name],[Storage].[Id],[Storage].Article," +
                                      "[Storage].[Count],[Storage].Price,[Storage].PurchasePrice,[Storage].[Status] " +
                                      "from [Storage] where [Storage].Article=@Article ";
                    cmd.Parameters.AddWithValue("Article", item.Item.Article);
                    cmd.Parameters.AddWithValue("Name", item.Item.Name);
                    cmd.Parameters.AddWithValue("UserId", item.User.Id);
                    cmd.Parameters.AddWithValue("Id", item.Item.Id);
                    cmd.Parameters.AddWithValue("Count", item.Item.Count);
                    cmd.Parameters.AddWithValue("Price", item.Item.Price);
                    cmd.Parameters.AddWithValue("PurchasePrice", item.Item.PurchasePrice ?? 1);

                    cmd.Parameters.AddWithValue("Status", Status.Active);


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        //cmd.ExecuteNonQuery(); 
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                res.Item.Name = reader.GetString(0);
                                res.Item.Id = reader.GetInt32(1);
                                res.Item.Article = reader.GetInt32(2);
                                res.Item.Count = reader.GetInt32(3);
                                res.Item.Price = reader.GetDecimal(4);
                                res.Item.PurchasePrice = -1;//reader.GetDecimal(5);
                                res.Item.Status = reader.GetInt32(6);
                                //return res;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        res.Error = "error_update_item";
                        res.Message = e.Message;
                        res.Item.Id = -1;
                    }

                    if (res.Item.Id != -1 && item.Item.Id.HasValue)
                    {                      

                        using SqlCommand statusCmd = con.CreateCommand();
                        statusCmd.CommandText =
                            "SELECT [Storage].[Status], [Status].[Name] " +
                            "FROM [Storage] INNER JOIN [Status] ON [Status].Id = [Storage].[Status] " +
                            "WHERE [Storage].Id = @ItemId";
                        statusCmd.Parameters.AddWithValue("ItemId", item.Item.Id.Value);
                        SqlDataReader statusReader = await statusCmd.ExecuteReaderAsync();
                        if (statusReader.Read())
                        {
                            res.Item.Status = statusReader.GetInt32(0);
                            res.Item.StatusName = statusReader.IsDBNull(1) ? string.Empty : statusReader.GetString(1);
                        }
                    }

                }
                con.Close();
            }
            return res;
        }

        public Task<bool> WriteOffItem(string data)
        {
            throw new NotImplementedException();
        }
    }
}
