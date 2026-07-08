using System.Data.SqlClient;
using Autofac.Core;
using CRM.Model;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.SqlServer.Server;

namespace CRM.Repositories.Implementations
{
    public class CompanyRepository : BaseRepository, ICompanyRepository
    {

        public CompanyRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<CreateCompanyResponse> CreateCompany(CompanyDto company, UserDto user)
        {
            CreateCompanyResponse response = new CreateCompanyResponse()
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
                        if (await CreateTrigger(company) == "Fail!")
                        {
                            response.Message = "Create trigger fail!";
                            response.Error = "error_create_trigger_fail";
                            response.Company.Id = -1;
                        }
                        response.Message = "Created new company!";
                        response.Error = "none";
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
                    cmd.CommandText = "use [CRM] SELECT * FROM [Company] WHERE [Company].CreatedByUserId=@UserId ";
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

        public Task<string> SetRole(User user)
        {
            throw new NotImplementedException();
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
