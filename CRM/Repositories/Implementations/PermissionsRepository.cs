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

        public async Task<ICollection<string>> GetEmployeePermissions(string companyName, int employeeId)
        {
            List<string> permissions = new List<string>();

            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT [Permission].[Name] FROM [Permissions],[Permission] " +
                        "WHERE [Permissions].EmployeeId=@EmployeeId AND [Permissions].PermissionId=[Permission].Id";
                    cmd.Parameters.AddWithValue("EmployeeId", employeeId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        permissions.Add(reader.GetString(0));
                    }
                }
                con.Close();
            }

            return permissions;
        }

        public async Task SaveEmployeePermissions(string companyName, int employeeId, ICollection<string> permissions)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();

                using (SqlCommand deleteCmd = con.CreateCommand())
                {
                    deleteCmd.CommandText = "DELETE FROM [Permissions] WHERE EmployeeId=@EmployeeId";
                    deleteCmd.Parameters.AddWithValue("EmployeeId", employeeId);
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                foreach (var permission in permissions.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    using SqlCommand insertCmd = con.CreateCommand();
                    insertCmd.CommandText =
                        "INSERT INTO [Permissions] (EmployeeId, PermissionId) " +
                        "SELECT @EmployeeId, Id FROM [Permission] WHERE [Name]=@PermissionName";
                    insertCmd.Parameters.AddWithValue("EmployeeId", employeeId);
                    insertCmd.Parameters.AddWithValue("PermissionName", permission);
                    await insertCmd.ExecuteNonQueryAsync();
                }

                con.Close();
            }
        }
    }
}
