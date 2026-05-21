using Microsoft.AspNetCore.Authorization;

namespace CRM.Services.Auth
{
    public class PermissionRequirements:IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirements(string permission)
        {
            Permission=permission;
        }
    }
}
