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
        private readonly IUserRepository _userRepository;

        public EmployeeServices(IEmployeeRepository employeeRepository, IUserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

        public async Task<EmployeeResponse> CreateEmployee(EmployeeModel model)
        {
            var creatorRole = model.User?.Role ?? string.Empty;
            var targetRole = model.Employee?.Role ?? string.Empty;

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

            var newUser = new UserDto
            {
                UserName = model.Employee.UserName,
                FirstName = model.Employee.FirstName,
                LastName = model.Employee.LastName,
                Password = string.IsNullOrWhiteSpace(model.Employee.Password)
                    ? model.Employee.UserName + "123"
                    : model.Employee.Password
            };

            var createdUser = await _userRepository.CreateUser(newUser);
            if (createdUser.Id == -1)
            {
                return new EmployeeResponse
                {
                    Message = "Username already exists.",
                    Error = "error_username_exists",
                    Employee = new Employee { Id = -1 }
                };
            }

            var linked = await _employeeRepository.LinkCompanyRole(
                createdUser.Id ?? -1,
                model.Company.Id,
                targetRole);

            if (!linked)
            {
                return new EmployeeResponse
                {
                    Message = "Failed to assign company role.",
                    Error = "error_role_assignment_failed",
                    Employee = new Employee { Id = -1 }
                };
            }

            return await _employeeRepository.CreateEmployee(
                model.Company,
                model.Employee,
                model.User,
                createdUser.Id ?? -1);
        }

        public async Task<EmployeeResponse> UpdateEmployee(EmployeeModel model)
        {
            var creatorRole = model.User?.Role ?? string.Empty;
            var targetRole = model.Employee?.Role ?? string.Empty;

            if (!CanAssignRole(creatorRole, targetRole))
            {
                return new EmployeeResponse
                {
                    Message = "You do not have permission to assign this role.",
                    Error = "error_role_not_allowed",
                    Employee = new Employee { Id = -1 }
                };
            }

            return await _employeeRepository.UpdateEmployee(model.Company, model.Employee, model.User);
        }

        public async Task<Task> DeleteEmployee(EmployeeModel model)
        {
            return await _employeeRepository.DeleteEmployee(model.Company, model.Employee, model.User);
        }

        public async Task<ICollection<Employee>> GetAll(CompanyDto company, int status)
        {
            return await _employeeRepository.GetAll(company, status);
        }

        private static bool CanAssignRole(string creatorRole, string targetRole)
        {
            if (string.Equals(targetRole, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(creatorRole, Roles.Ceo, StringComparison.OrdinalIgnoreCase);
            }

            if (string.Equals(targetRole, Roles.Employee, StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(creatorRole, Roles.Admin, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(creatorRole, Roles.Ceo, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
