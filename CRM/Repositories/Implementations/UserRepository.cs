

using System.Data.SqlClient;

namespace CRM.Repositories.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<string> CreateUser(string username, string password, string firstName, string lastName)
        {
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "use [CRM] IF NOT EXISTS (SELECT 1 FROM [User] WHERE [User].UserName=@UserName) " +
                                      "BEGIN " +
                                      "INSERT INTO [User] ([User].UserName, [User].[Password],[User].FirstName,[User].LastName) " +
                                      "VALUES (@UserName, @Password,@FirstName,@LastName); " +
                                      "SELECT 'Создано' AS Result; " +
                                      "END " +
                                      "ELSE BEGIN " +
                                      "SELECT 'Такой пользователь уже существует' AS Result; " +
                                      "END ";
                    cmd.Parameters.AddWithValue("UserName", username);
                    cmd.Parameters.AddWithValue("Password", password);
                    cmd.Parameters.AddWithValue("FirstName", firstName);
                    cmd.Parameters.AddWithValue("LastName", lastName);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string res = reader.GetString(0);
                            return res;
                        }
                    }
                    con.Close();
                }
            }

            return "Ошибка подключения";
        }

        public async Task<string> Login(string username, string password)
        {
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "use [CRM] SELECT [User].UserName FROM [User] WHERE [User].UserName=@UserName and [User].Password=@Password ";

                    cmd.Parameters.AddWithValue("UserName", username);
                    cmd.Parameters.AddWithValue("Password", password);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string res = reader.GetString(0);

                            return "Пользователь " + res;
                        }
                    }
                    else
                    {
                        return "Неверные данные для входа";
                    }
                    con.Close();
                }
            }

            return "Ошибка подключения";
        }
    }
}
