using System.Data.SqlClient;
using CRM.Model;

namespace CRM.Repositories.Implementations
{
    public class PermissionsRepository : BaseRepository, IPermissionsRepository
    {
        public PermissionsRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<ICollection<Permission>> GetPermissions(string userId, string companyName)
        {
            ICollection<Permission> permissions = new List<Permission>();
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "declare @eid as int " +
                        "set @eid = (select Employee.Id from Employee where UserId = @userId) " +
                        "select [Permission].[Name] from [Permissions],[Permission] " +
                        "where [Permissions].EmployeeId=@eid and [Permissions].PermissionId=[Permission].Id ";                        
                    cmd.Parameters.AddWithValue("userId", userId);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    try
                    {
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
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
                con.Close();
            }
            return permissions;
        }
    }
}
