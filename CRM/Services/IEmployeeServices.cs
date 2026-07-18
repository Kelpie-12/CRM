using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface IEmployeeServices
    {
        Task<EmployeeResponse> CreateEmployee(EmployeeModel model);
        Task<EmployeeResponse> UpdateEmployee(EmployeeModel model);
        Task<Task> DeleteEmployee(EmployeeModel model);
        Task<ICollection<Employee>> GetAll(CompanyDto company, int status);
    }
}
