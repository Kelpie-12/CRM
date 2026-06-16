
using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;

namespace CRM.Repositories.Implementations
{
    public class SalesRepository : BaseRepository, ISalesRepository
    {
        public SalesRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<string> CreateSales(CreateSaleModel model)
        {
            string res = "Ошибка создания продажи";
            using (SqlConnection con = CreateConnection(model.Company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "declare @countItem int " +
                        "set @countItem=(select [Storage].[Count] from Storage where Storage.Id=@ItemId) " +
                        "if(@countItem > @Count) " +
                        "begin " +
                            "INSERT INTO [dbo].[Sales] ([EmployeeId],[Status],[ItemId],[ItemArticle],[Count],[ClientId]) " +
                            "VALUES (@EmployeeId,@Status,@ItemId, " +
                            "(select [Storage].[Article] from Storage where Storage.Id=1),@Count,1) " +
                            "select 'Успешно' as Result " +
                            "end " +
                        "else " +
                            "select 'Недостаточно товара' as Result";
                    cmd.Parameters.AddWithValue("@EmployeeId", model.Employee.Id);
                    cmd.Parameters.AddWithValue("@ItemId", model.Item.Id);
                    cmd.Parameters.AddWithValue("@Count", model.Item.Count);

                    cmd.Parameters.AddWithValue("Status", 1);


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            res = reader.GetString(0);
                        }
                    }
                }
                con.Close();
            }

            return res;
        }

        public Task<bool> CreateSalesForClient(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeleteSales(int saleId, CompanyDto company)
        {
            string res = "Ошибка редактирования продажи";
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "";
                    //cmd.Parameters.AddWithValue("@EmployeeId", model.Employee.Id);
                    //cmd.Parameters.AddWithValue("@ItemId", model.Item.Id);
                    //cmd.Parameters.AddWithValue("@Count", model.Item.Count);

                    cmd.Parameters.AddWithValue("Status", 1);


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            res = reader.GetString(0);
                        }
                    }
                }
                con.Close();
            }

            return res;
        }

        public Task<string> GetInfoListSales(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoSales(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoSalesByClient(string data)
        {
            throw new NotImplementedException();
        }
    }
}
