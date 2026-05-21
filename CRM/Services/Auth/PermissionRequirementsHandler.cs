using System.Security.Claims;
using CRM.Repositories;
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
            using var scope = _serviceScopeFactory.CreateScope();
            var _userRepository = scope.ServiceProvider.GetService<IUserRepository>();
            var permissions = await _userRepository.GetPermissions(userName.Value);
            if (permissions.Any(x=>x.Name==requirement.Permission)) 
            {
                context.Succeed(requirement);
            }

            return  Task.CompletedTask;

        }
    }
}
