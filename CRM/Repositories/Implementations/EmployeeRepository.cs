using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;

namespace CRM.Repositories.Implementations
{
    public class EmployeeRepository : BaseRepository, IEmployeeRepository
    {
        private readonly ICompanyRepository _companyRepository;
        public EmployeeRepository(IConfiguration configuration, ICompanyRepository companyRepository) : base(configuration)
        {
            _companyRepository = companyRepository;

        }

        public async Task<EmployeeResponse> CreateEmployee(CompanyDto company, EmployeeDto employee, UserDto user)
        {
            EmployeeResponse response = new EmployeeResponse()
            {
                Error = "none",
                Message = "Employee was created!",
                Employee = new Employee()
            };

            UserDto addEmployee  = await GetUserIdByUsername(employee.UserName);
            if (addEmployee.Id == -1)
            {
                response.Message = "Username not exist";
                response.Error = "error_username_not_exist";
                response.Employee.Id = -1;
                return response;
            }
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
                        "INSERT INTO [Employee] ([FirstName],[LastName],[UserId],[RoleId],[Status],CreatedByUserId) " +
                        "VALUES (@FirstName,@LastName,@UserId,@RoleId,@Status,@CUI); " +
                        "SELECT Employee.Id,Employee.FirstName,Employee.LastName,Employee.CreatedAt,Employee.Status,[Role].[Name],Employee.UserId " +
                        "FROM [Employee] " +
                        "INNER JOIN [Role] ON [Role].Id = Employee.RoleId " +
                        "WHERE Employee.UserId=@UserId " +                        
                        "END " +
                        "ELSE BEGIN " +
                        "SELECT 'Employee already exist' AS Result; " +
                        "END ";

                    cmd.Parameters.AddWithValue("FirstName", addEmployee.FirstName);
                    cmd.Parameters.AddWithValue("LastName", addEmployee.LastName);
                    cmd.Parameters.AddWithValue("UserId", addEmployee.Id);
                    cmd.Parameters.AddWithValue("CUI",user.Id);
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

                    if (response.Employee.Id != -1)
                    {
                        await _companyRepository.SetRole(company, employee, addEmployee);
                    }
                }
                con.Close();
            }
            return response;
        }
        private async Task<UserDto> GetUserIdByUsername(string username)
        {
            UserDto res = new UserDto()
            {
                Id = -1
            };
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select [User].[Id],[User].FirstName,[User].LastName,[User].UserName from [User] where [User].UserName=@username";
                    cmd.Parameters.AddWithValue("@username", username);
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader.IsDBNull(0))
                                {
                                    res.Id = -1;
                                }
                                else
                                {
                                    res.Id =reader.GetInt32(0);
                                    res.FirstName = reader.GetString(1);
                                    res.LastName = reader.GetString(2);
                                    res.UserName = reader.GetString(3);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        res.Id = -1;                     
                    }
                }
                con.Close();
            }
            return res;
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
