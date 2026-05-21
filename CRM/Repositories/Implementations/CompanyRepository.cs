using System.Data.SqlClient;
using Autofac.Core;
using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Repositories.Implementations
{
    public class CompanyRepository : BaseRepository, ICompanyRepository
    {

        public CompanyRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<string> CreateCompany(CompanyDto company, UserDto user)
        {
            string res = "Ошибка в создание компании";
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

                    SqlParameter p= cmd.CreateParameter();
                    p.ParameterName = "result";
                    p.Value =res;
                    p.DbType= System.Data.DbType.String;
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
                    res =(string) cmd.Parameters["result"].Value;

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
