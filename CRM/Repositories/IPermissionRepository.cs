using CRM.Model;

namespace CRM.Repositories
{
    public interface IPermissionsRepository
    {
        Task<ICollection<Permission>> GetPermissions(string userName,string companyName);
    }
}
