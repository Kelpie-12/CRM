using System.Security.Claims;
using CRM.Repositories;
using CRM.Repositories.Implementations;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Services.Auth
{
    public class PermissionRequirementsHandler(IServiceScopeFactory _serviceScopeFactory) : AuthorizationHandler<PermissionRequirements>
    {

        protected override async Task<Task> HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirements requirement)
        {
            var userName = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userId = context.User.Claims.FirstOrDefault(x => x.Type == "id");
            var companyName = context.User.Claims.FirstOrDefault(x => x.Type == "selectedCompany");
            using var scope = _serviceScopeFactory.CreateScope();
            var _permissionRepository = scope.ServiceProvider.GetService<IPermissionsRepository>();
            if (userName == null)
            {

            }
            if (companyName != null)
            {
                var permissions = await _permissionRepository.GetPermissions(userId.Value, companyName.Value);

                if (permissions.Any(x => x.Name == requirement.Permission))
                {
                    context.Succeed(requirement);
                }
            }
            // var permissions = await _userRepository.GetPermissions(userName.Value);

            return Task.CompletedTask;

        }
    }
}
