using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SeniorLearnV3.Data.Identity
{
    public class ActiveRolePolicy : IAuthorizationRequirement { }

    public class ActiveRoleHandler : AuthorizationHandler<ActiveRolePolicy>
    {
        private readonly UserManager _userManager;

        public ActiveRoleHandler(UserManager userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveRolePolicy requirement)
        {
            var roleNames = context.Requirements
                                       .OfType<RolesAuthorizationRequirement>()
                                       .First().AllowedRoles.Select(r => r.Trim().ToUpper()).ToArray();
            if (!_userManager.UserHasActiveRoleAsync(context.User, roleNames).GetAwaiter().GetResult())
            {
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
