using System.Data.SqlClient;
using CRM.Model;

namespace CRM.Repositories.Implementations
{
    public class CompanyRepository : BaseRepository, ICompanyRepository
    {

        public CompanyRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<string> CreateCompany(Company company, User user)
        {
            string res = "Ошибка в создание компании";
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE [name] = @companyName) " +
                       "BEGIN " +
                       "   CREATE DATABASE @companyName;" +
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

                    // не верный запрос
                    cmd.Parameters.AddWithValue("userId", user.Id);
                    cmd.Parameters.AddWithValue("companyName", company.Name);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

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
