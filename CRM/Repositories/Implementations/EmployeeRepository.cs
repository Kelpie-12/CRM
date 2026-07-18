using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;

namespace CRM.Repositories.Implementations
{
    public class EmployeeRepository : BaseRepository, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<EmployeeResponse> CreateEmployee(CompanyDto company, EmployeeDto employee, UserDto user, int newUserId)
        {
            EmployeeResponse response = new EmployeeResponse()
            {
                Error = "none",
                Message = "Employee was created!",
                Employee = new Employee()
            };

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "IF NOT EXISTS (SELECT 1 FROM [Employee] WHERE [Employee].UserId=@UserId) " +
                        "BEGIN " +
                        "DECLARE @RoleId INT; " +
                        "SET @RoleId = (SELECT TOP 1 [Role].Id FROM [Role] WHERE [Role].[Name]=@RoleName); " +
                        "INSERT INTO [Employee] ([FirstName],[LastName],[UserId],[RoleId],[Status]) " +
                        "VALUES (@FirstName,@LastName,@UserId,@RoleId,@Status); " +
                        "SELECT Employee.Id,Employee.FirstName,Employee.LastName,Employee.CreatedAt,Employee.Status,[Role].[Name],Employee.UserId " +
                        "FROM [Employee] " +
                        "INNER JOIN [Role] ON [Role].Id = Employee.RoleId " +
                        "WHERE Employee.UserId=@UserId " +
                        "END " +
                        "ELSE BEGIN " +
                        "SELECT 'Employee already exist' AS Result; " +
                        "END ";

                    cmd.Parameters.AddWithValue("FirstName", employee.FirstName);
                    cmd.Parameters.AddWithValue("LastName", employee.LastName);
                    cmd.Parameters.AddWithValue("UserId", newUserId);
                    cmd.Parameters.AddWithValue("RoleName", employee.Role);
                    cmd.Parameters.AddWithValue("Status", Status.Active);

                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader.FieldCount == 1 && reader.GetString(0) == "Employee already exist")
                                {
                                    response.Message = reader.GetString(0);
                                    response.Employee.Id = -1;
                                    response.Error = "error_employee_already_exist";
                                }
                                else
                                {
                                    response.Employee.Id = reader.GetInt32(0);
                                    response.Employee.FirstName = reader.GetString(1);
                                    response.Employee.LastName = reader.GetString(2);
                                    response.Employee.CreatedAt = reader.GetDateTime(3);
                                    response.Employee.Status = reader.GetInt32(4);
                                    response.Employee.Role = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                                    response.Employee.UserId = reader.GetInt32(6);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Message = e.Message;
                        response.Error = "error_create_employee";
                        response.Employee.Id = -1;
                    }
                }
                con.Close();
            }

            return response;
        }

        public async Task<bool> LinkCompanyRole(int userId, int companyId, string roleName)
        {
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "USE [CRM] " +
                        "IF NOT EXISTS (SELECT 1 FROM CompanyRole WHERE UserId=@UserId AND CompanyId=@CompanyId) " +
                        "BEGIN " +
                        "DECLARE @RoleId INT; " +
                        "SET @RoleId = (SELECT TOP 1 Id FROM [Role] WHERE [Name]=@RoleName); " +
                        "INSERT INTO CompanyRole (UserId, CompanyId, [Role]) VALUES (@UserId, @CompanyId, @RoleId); " +
                        "SELECT 1 AS Result; " +
                        "END " +
                        "ELSE BEGIN SELECT 0 AS Result; END";

                    cmd.Parameters.AddWithValue("UserId", userId);
                    cmd.Parameters.AddWithValue("CompanyId", companyId);
                    cmd.Parameters.AddWithValue("RoleName", roleName);

                    object? result = await cmd.ExecuteScalarAsync();
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
        }

        public async Task<EmployeeResponse> UpdateEmployee(CompanyDto company, EmployeeDto employee, UserDto user)
        {
            EmployeeResponse response = new EmployeeResponse()
            {
                Error = "none",
                Message = "Employee was updated!",
                Employee = new Employee()
            };

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "DECLARE @RoleId INT; " +
                        "SET @RoleId = (SELECT TOP 1 [Role].Id FROM [Role] WHERE [Role].[Name]=@RoleName); " +
                        "UPDATE [Employee] " +
                        "SET [FirstName]=@FirstName, [LastName]=@LastName, [RoleId]=@RoleId " +
                        "WHERE [Employee].Id=@EmployeeId; " +
                        "SELECT Employee.Id,Employee.FirstName,Employee.LastName,Employee.CreatedAt,Employee.Status,[Role].[Name],Employee.UserId " +
                        "FROM [Employee] " +
                        "INNER JOIN [Role] ON [Role].Id = Employee.RoleId " +
                        "WHERE Employee.Id=@EmployeeId";

                    cmd.Parameters.AddWithValue("EmployeeId", employee.Id);
                    cmd.Parameters.AddWithValue("FirstName", employee.FirstName);
                    cmd.Parameters.AddWithValue("LastName", employee.LastName);
                    cmd.Parameters.AddWithValue("RoleName", employee.Role);

                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                response.Employee.Id = reader.GetInt32(0);
                                response.Employee.FirstName = reader.GetString(1);
                                response.Employee.LastName = reader.GetString(2);
                                response.Employee.CreatedAt = reader.GetDateTime(3);
                                response.Employee.Status = reader.GetInt32(4);
                                response.Employee.Role = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                                response.Employee.UserId = reader.GetInt32(6);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        response.Message = e.Message;
                        response.Error = "error_update_employee";
                        response.Employee.Id = -1;
                    }
                }
                con.Close();
            }

            return response;
        }

        public async Task<Task> DeleteEmployee(CompanyDto company, EmployeeDto employee, UserDto user)
        {
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "UPDATE [Employee] SET [Status]=@Status WHERE [Employee].Id=@EmployeeId";
                    cmd.Parameters.AddWithValue("EmployeeId", employee.Id);
                    cmd.Parameters.AddWithValue("Status", Status.Deleted);
                    await cmd.ExecuteNonQueryAsync();
                }
                con.Close();
            }

            return Task.CompletedTask;
        }

        public async Task<ICollection<Employee>> GetAll(CompanyDto company, int status)
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT Employee.Id,Employee.FirstName,Employee.LastName,Employee.CreatedAt,Employee.Status, [Role].Name,Employee.UserId " +
                        "FROM [Employee],[Role] WHERE Employee.Status=@Status and Role.Id=Employee.RoleId";
                    cmd.Parameters.AddWithValue("Status", status);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee()
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3),
                                Status = reader.GetInt32(4),
                                Role = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                UserId = reader.GetInt32(6)
                            });
                        }
                    }
                }
                con.Close();
            }

            return employees;
        }
    }
}
