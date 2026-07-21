using CRM.Model;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface IEmployeeRepository
    {
        Task<EmployeeResponse> CreateEmployee(CompanyDto company, EmployeeDto employee, UserDto user);
      //  Task<bool> LinkCompanyRole(int userId, int companyId, string roleName);
        Task<EmployeeResponse> UpdateEmployee(CompanyDto company, EmployeeDto employee, UserDto user);
        Task<Task> DeleteEmployee(CompanyDto company, EmployeeDto employee, UserDto user);
        Task<ICollection<Employee>> GetAll(CompanyDto company, int status);
    }
}
