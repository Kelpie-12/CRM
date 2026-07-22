
using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;

namespace CRM.Repositories.Implementations
{
    public class SalesRepository : BaseRepository, ISalesRepository
    {
        public SalesRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<SaleResponse> CancelSale(SaleCancelModel sale)
        {
            SaleResponse response = new SaleResponse()
            {
                Message = "Sale cancel!",
                Error = "none",
                Sales = new Sales() 
            };
            using (SqlConnection con = CreateConnection(sale.Company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "Cancel_sale";

                    cmd.Parameters.AddWithValue("@SaleId", sale.Sale.Id);         
                    cmd.Parameters.AddWithValue("@CashboxName", "Main");
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        //cmd.ExecuteNonQuery(); 
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                response.Sales.Id = reader.GetInt32(0);
                                if (response.Sales.Id != -1)
                                {
                                    response.Sales.EmployeeId = reader.GetInt32(1);
                                    response.Sales.Article = reader.GetInt32(2);
                                    response.Sales.Count = reader.GetInt32(3);
                                    response.Sales.SalesDate = reader.GetDateTime(4);
                                    response.Sales.Status = reader.GetInt32(5);
                                    response.Sales.ItemId = reader.GetInt32(6);
                                    response.Sales.TransitionId = reader.GetInt32(7);
                                }
                                else
                                {
                                    response.Message = "Insufficient quantity!";
                                    response.Error = "error_cancel_sale";
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Message = e.Message;
                        response.Error = "error_cancel_sale";
                        response.Sales.Id = -1;
                    }
                }
              

                con.Close();
            }


            return response;
        }

        public async Task<SaleResponse> CreateSales(SaleModel model)
        {
            SaleResponse response = new SaleResponse()
            {
                Message = "Sale created!",
                Error = "none",
                Sales = new Sales()
            };

            using (SqlConnection con = CreateConnection(model.Company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "New_Sale";
                    
                    cmd.Parameters.AddWithValue("@UserId", model.Employee.Id);
                    cmd.Parameters.AddWithValue("@ItemId", model.Item.Id);
                    cmd.Parameters.AddWithValue("@Count", model.Item.Count);
                    cmd.Parameters.AddWithValue("@ClientId", model.Client?.Id ?? 1);
                    cmd.Parameters.AddWithValue("@CashboxName", "Main");


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        //cmd.ExecuteNonQuery(); 
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                response.Sales.Id = reader.GetInt32(0);
                                if (response.Sales.Id != -1)
                                {
                                    response.Sales.EmployeeId = reader.GetInt32(1);
                                    response.Sales.Article = reader.GetInt32(2);
                                    response.Sales.Count = reader.GetInt32(3);
                                    response.Sales.SalesDate = reader.GetDateTime(4);
                                    response.Sales.Status = reader.GetInt32(5);
                                    response.Sales.ItemId = reader.GetInt32(6);
                                    response.Sales.TransitionId = reader.GetInt32(7);
                                }
                                else
                                {
                                    response.Message = "Insufficient quantity!";
                                    response.Error = "error_create_sale_no_count";
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Message = e.Message;
                        response.Error = "error_create_sale";
                        response.Sales.Id = -1;
                    }
                }


                con.Close();
            }

            return response;
        }

        public Task<bool> CreateSalesForClient(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<Task> DeleteSales(int saleId, CompanyDto company)
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

            return Task.CompletedTask;
        }

        public async Task<ICollection<Sales>> GetAll(CompanyDto company,int status)
        {
            ICollection<Sales> sales = new List<Sales>();
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select Sales.Id,Sales.EmployeeId," +
                        "Sales.ItemArticle,Sales.[Count],Sales.CreatedAt,Sales.Status,Sales.ItemId,Sales.TransactionsId " +
                        "from Sales where Sales.Status=@Status";
                    //cmd.Parameters.AddWithValue("@EmployeeId", model.Employee.Id);
                    //cmd.Parameters.AddWithValue("@ItemId", model.Item.Id);
                    //cmd.Parameters.AddWithValue("@Count", model.Item.Count);

                    cmd.Parameters.AddWithValue("Status",status);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            sales.Add(new Sales()
                            {
                                Id= reader.GetInt32(0),
                                EmployeeId = reader.GetInt32(1),
                                Article = reader.GetInt32(2),
                                Count = reader.GetInt32(3),
                                SalesDate = reader.GetDateTime(4),
                                Status = reader.GetInt32(5),
                                ItemId = reader.GetInt32(6),
                                TransitionId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                            });
                        }
                    }
                }
                con.Close();
            }

            return sales;
        }

        public Task<string> GetInfoSales(string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInfoSalesByClient(string data)
        {
            throw new NotImplementedException();
        }

        public Task<SaleResponse> UpdateSales(SaleModel model)
        {
            throw new NotImplementedException();
        }
    }
}
