using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Transactions;
using System.Xml.Linq;
using Autofac.Core;
using CRM.Model;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.SqlServer.Server;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRM.Repositories.Implementations
{
    public class CompanyRepository : BaseRepository, ICompanyRepository
    {

        public CompanyRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<CompanyResponse> CreateCompany(CompanyDto company, UserDto user)
        {
            CompanyResponse response = new CompanyResponse()
            {
                Message = "Fail!",
                Error = "error_create_company",
                Company = new CompanyDto() { Name = company.Name }
            };
            CompanyDto tmp = new CompanyDto();
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    //уязвимость при создание базы данных
                    /*
                     * cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE [name] = @companyName) " +
                       "BEGIN " +
                       $"   CREATE DATABASE {company.Name};" +
                       "   use CRM" +
                       "   insert into Company ([Name],[Status],CreatedByUserId) " +
                       "   values (@companyName,1,@userId);" +
                       "   declare @companyId as int; " +
                       "   set @companyId = (select Company.Id from Company where Company.[Name]=@companyName)" +
                       "   insert into CompanyRole (UserId,CompanyId,[Role]) " +
                       "   values (@userId,@companyId,'1')" +
                       " select 'Успешно' as Result " +
                       "END; " +
                       "else  " +
                       "   select 'Такая компания уже есть' as Result";
                    */
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "CreateCompany";
                    cmd.Parameters.AddWithValue("userId", user.Id);
                    cmd.Parameters.AddWithValue("companyName", company.Name);

                    SqlParameter p = cmd.CreateParameter();
                    p.ParameterName = "result";
                    p.Value = response.Message;
                    p.DbType = System.Data.DbType.String;
                    p.Size = 200;
                    p.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(p);

                    //cmd.Parameters.AddWithValue("result", res).Direction= System.Data.ParameterDirection.Output;              
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //if (reader.HasRows)
                    //{
                    //    while (reader.Read())
                    //    {
                    //        res = reader.GetString(0);
                    //    }
                    //}
                    response.Message = (string)cmd.Parameters["result"].Value;
                    if (response.Message != "Fail! Company already exist")
                    {
                        response.Company.Id = Convert.ToInt32(response.Message);
                        if (await CreateProcedureNewSale(company) == "Fail!")
                        {
                            response.Message = "Create NewSale procedure fail!";
                            response.Error = "error_create_procedure_fail";
                            response.Company.Id = -1;

                        }
                        else
                        {
                            response.Message = "Created new company!";
                            response.Error = "none";
                        }
                        if (await CreateProcedureCancelSale(company) == "Fail")
                        {
                            response.Message = "Create CancelSale procedure fail!";
                            response.Error = "error_create_procedure_fail";
                            response.Company.Id = -1;
                        }
                        else
                        {
                            response.Message = "Created new company!";
                            response.Error = "none";
                        }
                    }
                    else
                    {
                        tmp.Id = -1;
                    }

                }
                con.Close();
            }

            return response;
        }
        private async Task<string> CreateTrigger(CompanyDto company)
        {
            string res = "Fail!";
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText =
                            "CREATE TRIGGER trg_AfterInsert_Sales " +
                            "ON Sales " +
                            "AFTER INSERT AS BEGIN " +
                            "SET NOCOUNT ON; " +
                            "declare @count as int " +
                            "set @count=(select [Count] from inserted) " +
                            "update Storage " +
                            "set Storage.[Count]=Storage.[Count]-@count " +
                            "where Storage.Id =(select ItemId from inserted)  " +
                            "END;";

                        await cmd.ExecuteNonQueryAsync();
                        res = "Success!";
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.Message);
                        return $"Error - {e.Message}";
                    }
                }
                con.Close();
            }
            return res;
        }

        private async Task<string> CreateProcedureNewSale(CompanyDto company)
        {
            string res = "Fail!";
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText =
                           "Create PROCEDURE New_Sale " +
                           "@Count as int," +
                           "@ItemId as int," +
                           "@UserId as int," +
                           "@ClientId as int," +
                           "@CashboxName as nvarchar(150) " +
                           "as begin " +
                           "begin TRANSACTION; " +
                           "declare @countItem int " +
                           "set @countItem=(select [Storage].[Count] from Storage where Storage.Id=@ItemId) " +
                           "if(@countItem > @Count) " +
                           " begin " +
                           " declare @eId int " +
                           " declare @Status int " +
                           " set @Status=(select [Status].[Id] from [Status] where [Status].[Name]='Active')" +
                           " set @eId =(select Id from Employee where UserId=@UserId) " +

                           " insert into FiscalDocumentNumber (ItemId,Count,OperationType,Amount,Status) " +
                           " values (@ItemId," +
                           "         @Count," +
                           "         (select Id from OperationType where OperationType.Name='Income')," +
                           "         @Count*(select Price from Storage where Storage.Id=@ItemId)," +
                           "         @Status)" +

                           " insert into Transactions (CashboxId,OperationType,Amount,FiscalDocumentNumber,[Status]) " +
                           " values ((select Id from Cashbox where Cashbox.Name=@CashboxName)," +
                           "         (select Id from OperationType where OperationType.Name='Income')," +
                           "         @Count*(select Price from Storage where Storage.Id=@ItemId)," +
                           "         (SELECT SCOPE_IDENTITY()),@Status);" +

                           " INSERT INTO [dbo].[Sales] ([EmployeeId],[Status],[ItemId],[ItemArticle],[Count],[ClientId],[TransactionsId]) " +
                           " VALUES (@eId," +
                           "         @Status," +
                           "         @ItemId," +
                           "         (select [Storage].[Article] from Storage where Storage.Id=@ItemId)," +
                           "         @Count," +
                           "         @ClientId," +
                           "         (SELECT SCOPE_IDENTITY()))" +

                           " select Sales.Id,Sales.EmployeeId,Sales.ItemArticle,Sales.[Count],Sales.CreatedAt,Sales.Status,Sales.ItemId,Sales.TransactionsId " +
                           " from Sales where Sales.Id=(SELECT SCOPE_IDENTITY())" +

                           " update Storage " +
                           " set Storage.[Count]=Storage.[Count]-@Count" +
                           " where Storage.Id =@ItemId " +//(select ItemId from Sales where Sales.Id=(SELECT SCOPE_IDENTITY()))

                           " declare @balanceNow as int " +
                           " set @balanceNow =(select Cashbox.Balance from Cashbox" +
                           "                   where Cashbox.Id =(select Id from Cashbox where Cashbox.Name=@CashboxName))" +

                           " update Cashbox " +
                           " set Cashbox.Balance=@balanceNow+ (@Count*(select Price from Storage where Storage.Id=@ItemId))" +
                           " where Cashbox.Id =(select Id from Cashbox where Cashbox.Name=@CashboxName)" +

                           " end  " +
                           "else " +
                           " select -1 as Result " +
                           "end; " +
                           "commit;";

                        await cmd.ExecuteNonQueryAsync();
                        res = "Success!";
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.Message);
                        return $"Error - {e.Message}";
                    }
                }
                con.Close();
            }
            return res;
        }

        private async Task<string> CreateProcedureCancelSale(CompanyDto company)
        {
            string res = "Fail!";
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText =
                        "Create PROCEDURE [dbo].[Cancel_Sale]                                                       " +
                        "@SaleId as int,                                                                                                      " +
                        "@CashboxName as nvarchar(150)                                                                                        " +
                        "as                                                                                                                   " +
                        "begin                                                                                                                " +
                        "	begin TRANSACTION;                                                                                                " +
                        "                                                                                                                     " +
                        "		declare @transId as int                                                                                       " +
                        "		declare @amount as int                                                                                        " +
                        "		declare @itemId as int                                                                                        " +
                        "		set @transId =(select Sales.TransactionsId from Sales where Sales.Id=@SaleId)                                 " +
                        "		set @amount =(select Amount from Transactions where Transactions.Id=@transId)                                 " +
                        "		set @itemId =(select Sales.ItemId from Sales where Sales.Id=@SaleId)                                          " +
                        "		" +
                        "       insert into FiscalDocumentNumber (ItemId,OperationType,Count,Amount,Status) " +
                        "       values (@itemId," +
                        "               (select Id from OperationType where OperationType.Name='Outcome')," +
                        "               (select Sales.[Count] from Sales where Sales.Id=@SaleId)," +
                        "               @amount," +
                        "               (select [Status].Id from [Status] where [Status].[Name]='Canceled'))  " +
                        "                                                                                                            " +
                        "		update Storage                                                                                                " +
                        "		set Storage.[Count]=Storage.[Count]+(select Sales.[Count] from Sales where Sales.Id=@SaleId)                       " +
                        "		where Storage.Id =(@itemId)  " +
                        "                                                                                 " +
                        "	    update Sales                     " +
                        "       set Sales.[Status]=(select [Status].Id from [Status] where [Status].[Name]='Canceled')" +
                        "       where Sales.Id =(@SaleId)   " +
                        "                                                                                                        " +
                        "		insert into Transactions (CashboxId,OperationType,Amount,FiscalDocumentNumber,[Status],CreatedAt)                       " +
                        "		values ((select Id from Cashbox where Cashbox.Name=@CashboxName),                                             " +
                        "				(select Id from OperationType where OperationType.Name='Outcome'),                                    " +
                        "				@amount,                                                                                              " +
                        "				(SELECT SCOPE_IDENTITY()),                                                                               " +
                        "				(select [Status].[Id] from [Status] where [Status].[Name]='Active')," +
                        "               GETDATE());                                 " +
                        "                                                                                                                     " +
                        "                                                                                                                     " +
                        "		 declare @balanceNow as int                                                                                   " +
                        "		 set @balanceNow = (select Cashbox.Balance                                                                    " +
                        "						  from Cashbox                                                                                " +
                        "						  where Cashbox.Id =(select CashboxId from Transactions                                       " +
                        "											 where Transactions.Id=@transId))                                         " +
                        "		                                                                                                              " +
                        "		 update Cashbox                                                                                               " +
                        "		 set Cashbox.Balance=@balanceNow - @amount                                                                    " +
                        "		 where Cashbox.Id =(select CashboxId from Transactions where Transactions.Id=@transId)                        " +
                        "		                                                                                                              " +
                        "		                                                                                                              " +
                        "		select Sales.Id,Sales.EmployeeId,Sales.ItemArticle,Sales.[Count],                                             " +
                        "       Sales.CreatedAt,Sales.Status,Sales.ItemId,TransactionsId                                                      " +
                        "		from Sales where Sales.Id=@SaleId                                                                             " +
                        "		                                                                                                              " +
                        "	                                                                                                                  " +
                        "	end  	                                                                                                          " +
                        "commit;";

                        await cmd.ExecuteNonQueryAsync();
                        res = "Success!";
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.Message);
                        return $"Error - {e.Message}";
                    }
                }
                con.Close();
            }
            return res;
        }

        public Task<string> DeleteCompany(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Company>> GetAllByUser(User user)
        {
            List<Company> res = new List<Company>();
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select Company.Id,Company.Name,Company.CreatedAt,Company.Status " +
                                      "from Company,CompanyRole " +
                                      "where Company.Id=CompanyRole.CompanyId and CompanyRole.UserId=@UserId";
                    // не верный запрос
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Company company = new Company();
                            company.Id = reader.GetInt32(0);
                            company.Name = reader.GetString(1);
                            company.CreatedAt = reader.GetDateTime(2);
                            company.Status = reader.GetInt32(3);
                            res.Add(company);
                        }
                    }

                }
                con.Close();
            }

            return res;
        }

        public async Task<string> SetRole(CompanyDto company, EmployeeDto employee, UserDto user)
        {
            string res = "Ok";
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "insert into CompanyRole (UserId,CompanyId,Role) " +
                        "values (@UserId,@CompanyId,(select Id from Role where [Name]=@Role))";
                    cmd.Parameters.AddWithValue("@UserId", user.Id);
                    cmd.Parameters.AddWithValue("@CompanyId", company.Id);
                    cmd.Parameters.AddWithValue("@Role", employee.Role);
                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        res = e.Message;
                    }
                }

            }
            return await Task.FromResult(res);
        }

        public Task<string> UpdateCompany(Company company)
        {
            throw new NotImplementedException();
        }

        public async Task<Company> GetById(int id, int userId)
        {
            Company company = new Company();
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "use [CRM] select Company.Id,Company.[Name],Company.CreatedAt,Company.[Status] from Company,CompanyRole " +
                        "where Company.Id=@Id and CompanyRole.UserId=@UserId ";

                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.AddWithValue("UserId", userId);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            company.Id = reader.GetInt32(0);
                            company.Name = reader.GetString(1);
                            company.CreatedAt = reader.GetDateTime(2);
                            company.Status = reader.GetInt32(3);
                        }
                    }

                }
                con.Close();
            }

            return company;

        }
    }
}
