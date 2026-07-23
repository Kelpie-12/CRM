using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;
using CRM.Repositories;

namespace CRM.Services.Implementations
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPermissionsRepository _permissionsRepository;

        public EmployeeServices(
            IEmployeeRepository employeeRepository,
            IPermissionsRepository permissionsRepository)
        {
            _employeeRepository = employeeRepository;
            _permissionsRepository = permissionsRepository;
        }

        public async Task<EmployeeResponse> CreateEmployee(EmployeeModel model)
        {
            string creatorRole = model.User?.Role ?? string.Empty;
            string targetRole = model.Employee?.Role ?? string.Empty;

            if (!CanAssignRole(creatorRole, targetRole))
            {
                return new EmployeeResponse
                {
                    Message = "You do not have permission to assign this role.",
                    Error = "error_role_not_allowed",
                    Employee = new Employee { Id = -1 }
                };
            }

            if (string.IsNullOrWhiteSpace(model.Employee?.UserName))
            {
                return new EmployeeResponse
                {
                    Message = "Username is required.",
                    Error = "error_username_required",
                    Employee = new Employee { Id = -1 }
                };
            }

            EmployeeResponse response = await _employeeRepository.CreateEmployee(
                model.Company,
                model.Employee,
                model.User);

            if (response.Employee?.Id != -1)
            {
                await _permissionsRepository.SaveEmployeePermissions(
                    model.Company.Name,
                    response.Employee.Id,
                    model.Employee.Permissions ?? new List<string>());

                response.Employee.Permissions = model.Employee.Permissions?.ToList() ?? new List<string>();
            }
            

            return response;
        }

        public async Task<EmployeeResponse> UpdateEmployee(EmployeeModel model)
        {
            string creatorRole = model.User?.Role ?? string.Empty;
            string targetRole = model.Employee?.Role ?? string.Empty;

            if (!CanAssignRole(creatorRole, targetRole))
            {
                return new EmployeeResponse
                {
                    Message = "You do not have permission to assign this role.",
                    Error = "error_role_not_allowed",
                    Employee = new Employee { Id = -1 }
                };
            }

            EmployeeResponse response = await _employeeRepository.UpdateEmployee(
                model.Company,
                model.Employee,
                model.User);

            if (response.Employee?.Id != -1)
            {
                await _permissionsRepository.SaveEmployeePermissions(
                    model.Company.Name,
                    response.Employee.Id,
                    model.Employee.Permissions ?? new List<string>());

                response.Employee.Permissions = model.Employee.Permissions?.ToList() ?? new List<string>();
            }

            return response;
        }

        public async Task<Task> DeleteEmployee(EmployeeModel model)
        {
            return await _employeeRepository.DeleteEmployee(model.Company, model.Employee, model.User);
        }

        public async Task<ICollection<Employee>> GetAll(CompanyDto company, int status)
        {
            ICollection<Employee> employees = await _employeeRepository.GetAll(company, status);

            foreach (Employee employee in employees)
            {
                employee.Permissions = (await _permissionsRepository.GetEmployeePermissions(
                    company.Name,
                    employee.Id)).ToList();
            }

            return employees;
        }

        private static bool CanAssignRole(string creatorRole, string targetRole)
        {
            if (string.Equals(targetRole, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(creatorRole, Roles.Ceo, StringComparison.OrdinalIgnoreCase);
            }

            if (string.Equals(targetRole, Roles.Ceo, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(targetRole, Roles.Employee, StringComparison.OrdinalIgnoreCase))
            {
                return IsAdminOrCeo(creatorRole);
            }

            return IsAdminOrCeo(creatorRole);
        }

        private static bool IsAdminOrCeo(string creatorRole)
        {
            return string.Equals(creatorRole, Roles.Admin, StringComparison.OrdinalIgnoreCase)
                || string.Equals(creatorRole, Roles.Ceo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
