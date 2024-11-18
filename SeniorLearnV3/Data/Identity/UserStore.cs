using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SeniorLearnV3.Data.Identity
{

    public class UserStore
    : UserStore<User, Role, ApplicationDbContext, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
    {
        public UserStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer)
        {

        }

        public DbSet<Role> Roles { get => Context.Set<Role>(); }
        public DbSet<UserRole> UserRoles { get => Context.Set<UserRole>(); }

        protected override Task<UserRole?> FindUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
        {
            return UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        public async Task AddToRoleAsync(User user, string normalizedRoleName, bool active = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value Cannot Be Null Or Empty", nameof(normalizedRoleName));
            }
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "RoleNotFound", normalizedRoleName));
            }
            UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id, Active = active });
        }
    }
}
