

using System.Data.SqlClient;
using CRM.Model;

namespace CRM.Repositories.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<User> CreateUser(User user)
        {
            string res = "";
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "use [CRM] IF NOT EXISTS (SELECT 1 FROM [User] WHERE [User].UserName=@UserName) " +
                                      "BEGIN " +
                                      "INSERT INTO [User] ([User].UserName, [User].[Password],[User].FirstName,[User].LastName) " +
                                      "VALUES (@UserName, @Password,@FirstName,@LastName); " +
                                      "SELECT 'Успех' AS Result; " +
                                      "END " +
                                      "ELSE BEGIN " +
                                      "SELECT 'Такой пользователь уже существует' AS Result; " +
                                      "END ";
                    cmd.Parameters.AddWithValue("UserName", user.UserName);
                    cmd.Parameters.AddWithValue("Password", user.HashPassword);
                    cmd.Parameters.AddWithValue("FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("LastName", user.LastName);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == "Успех")
                            {
                                return user;
                            }
                            //User tmp = new User();                           
                            //tmp.UserName = reader.GetString(0);
                            //tmp.Id = reader.GetInt32(1);
                            //tmp.HashPassword = reader.GetString(2);
                            //tmp.FirstName = reader.GetString(3);
                            //tmp.LastName = reader.GetString(4);
                            //tmp.CreatedAt = reader.GetDateTime(5);
                            //return tmp;
                        }
                    }
                }
                con.Close();
            }
            throw new Exception(res);
        }

        public async Task<User> Login(string username)
        {
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "use [CRM] SELECT * FROM [User] WHERE [User].UserName=@UserName ";

                    cmd.Parameters.AddWithValue("UserName", username);
                    //  cmd.Parameters.AddWithValue("Password", password);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            user.UserName = reader.GetString(0);
                            user.Id = reader.GetInt32(1);
                            user.HashPassword = reader.GetString(2);
                            user.FirstName = reader.GetString(3);
                            user.LastName = reader.GetString(4);
                            user.CreatedAt = reader.GetDateTime(5);
                            return user;
                        }
                    }


                }
                con.Close();
            }
            return null;
            //throw new Exception("Пользователя с таким логинам нет");

        }
    }
}
