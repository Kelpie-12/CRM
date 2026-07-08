using CRM.Model;

namespace CRM.Repositories.Implementations
{
    public interface IPermissionsRepository
    {
        Task<ICollection<Permission>> GetPermissions(string userName,string companyName);
    }
}
