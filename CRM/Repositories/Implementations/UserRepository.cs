

using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.DTO;

namespace CRM.Repositories.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<UserDto> CreateUser(UserDto user)
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
                                      "SELECT [User].Id FROM [User] WHERE [User].UserName=@UserName " +
                                      //"SELECT 'true' AS Result; " +
                                      "END " +
                                      "ELSE BEGIN " +
                                      "SELECT -1 AS Result; " +
                                      "END ";
                    cmd.Parameters.AddWithValue("UserName", user.UserName);
                    cmd.Parameters.AddWithValue("Password", user.Password);
                    cmd.Parameters.AddWithValue("FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("LastName", user.LastName);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetInt32(0) == -1)
                            {
                                return new UserDto() { Id = -1 };
                            }
                            user.Id = reader.GetInt32(0);                          
                            return user;
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

        public Task<User> DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

       /* public async Task<ICollection<Permission>> GetPermissions(string userName)
        {
            ICollection<Permission> permissions = new List<Permission>();
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "use [CRM] " +
                        "select [Permission].[Name] from [Permissions],[Permission],[User] " +
                        "where [Permissions].UserId=[User].Id and [Permissions].PermissionId=[Permission].Id " +
                        "and [User].Id=" +
                        "(select [User].Id from [User] where [User].UserName=@UserName)";
                    cmd.Parameters.AddWithValue("UserName", userName);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Permission permission = new Permission();
                            permission.Name = reader.GetString(0);
                            permissions.Add(permission);

                        }
                    }
                }
                con.Close();
            }
            return permissions;
        }
       */
        public Task<User> GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUser(int userId, int companyId)
        {
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "use [CRM] SELECT U.[UserName],U.[Id],U.[Password],U.[FirstName],U.[LastName],U.[CreatedAt],U.[Status],R.[Name] " +
                        "FROM [dbo].[User] as U,CompanyRole as CR, [Role] as R " +
                        "where U.[Id] = @UserId and CR.[Role] = R.Id and CR.[CompanyId] = @CompanyId";
                    cmd.Parameters.AddWithValue("UserId", userId);
                    cmd.Parameters.AddWithValue("CompanyId", companyId);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
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
                            user.Status = -1;//Convert.IsDBNull(reader.GetInt32(6))? -1: reader.GetInt32(6);
                            user.Role = reader.GetString(7);
                            return user;
                        }
                    }


                }
                con.Close();
            }
            return null;
            throw new NotImplementedException();
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

        public Task<User> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
