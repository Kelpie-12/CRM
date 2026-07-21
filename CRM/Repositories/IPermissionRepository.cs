using CRM.Model;

namespace CRM.Repositories
{
    public interface IPermissionsRepository
    {
        Task<ICollection<Permission>> GetPermissions(string userName, string companyName);
        Task<ICollection<string>> GetEmployeePermissions(string companyName, int employeeId);
        Task SaveEmployeePermissions(string companyName, int employeeId, ICollection<string> permissions);
    }
}
