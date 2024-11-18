using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SeniorLearnV3.Data.Identity
{
    public class RoleStore : RoleStore<Role, ApplicationDbContext, string, UserRole, IdentityRoleClaim<string>>
    {
        public RoleStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
        {
        }
    }
}
