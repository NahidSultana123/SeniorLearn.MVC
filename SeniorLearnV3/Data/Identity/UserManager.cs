using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SeniorLearnV3.Data.Identity
{
    public class UserManager : UserManager<User>
    {
        private readonly UserStore _store;

        public UserManager(
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger
        )
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _store = (UserStore)store;
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role, bool active = true, string notes = "")
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var normalizedRole = NormalizeName(role);
            if (await _store.IsInRoleAsync(user, normalizedRole, CancellationToken).ConfigureAwait(false))
            {
                return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(role));
            }
            await _store.AddToRoleAsync(user, normalizedRole, active, CancellationToken).ConfigureAwait(false);
            var result = await UpdateUserAsync(user).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var update = new RoleUpdate
                {
                    Active = active,
                    Timestamp = DateTime.Now,
                    Notes = $"Role: {normalizedRole} - {notes}",
                };
                if (user.Member != null)
                {
                    update.RenewaDate = user.Member.RenewalDate;

                }
                var roleId = (await _store.Roles.FirstOrDefaultAsync(r => r.NormalizedName!.Equals(normalizedRole)))!.Id;
                var userRole = await _store.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == roleId);
                userRole!.Updates.Add(update);
                await _store.Context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IdentityResult> ActivateOrDeactivateRoleAsync(User user, string role, bool active, string notes = "")
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            role = NormalizeName(role);
            var r = await _store.Roles.FirstOrDefaultAsync(r => r.NormalizedName!.Equals(role));

            if (r == null)
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidRoleName(role));
            }

            var ur = await _store.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == r.Id);
            if (ur == null)
            {
                return IdentityResult.Failed(ErrorDescriber.UserNotInRole(role));
            }
            if (ur.Active == active)
            {
                return IdentityResult.Failed(new[] { new IdentityError { Description = $"{user.UserName}'s activation status for the role {role} is already set to {(active ? "active" : "inactive")}" } });
            }

            ur.Active = active;


            await _store.Context.SaveChangesAsync().ConfigureAwait(false);


            var update = new RoleUpdate
            {
                Active = active,
                Timestamp = DateTime.Now,
                Notes = $"Role: {r.NormalizedName} - {notes}",
            };

            if (user.Member != null)
            {
                update.RenewaDate = user.Member.RenewalDate;
            }

            ur.Updates.Add(update);
            await _store.Context.SaveChangesAsync().ConfigureAwait(false);

            return IdentityResult.Success;
        }

        public Task<bool> UserHasActiveRoleAsync<T>(ClaimsPrincipal cp) where T : UserRole
          => _store.UserRoles.OfType<T>()
              .AnyAsync(ur => ur.User.UserName == cp.Identity!.Name && ur.Active);

        public Task<bool> IsActiveAdministratorAsync(ClaimsPrincipal cp)
            => _store.UserRoles.AnyAsync(ur => ur.User.UserName == cp.Identity!.Name && ur.Role.Name == "ADMINISTRATION" && ur.Active);

        public async Task<bool> UserHasActiveRoleAsync(ClaimsPrincipal principle, params string[] roleNames)
        {
            var user = await GetUserAsync(principle);
            if (user == null)
            {
                return false;
            }
            var roles = new List<Role>();
            foreach (var role in await _store.Roles.ToArrayAsync())
            {
                foreach (string rn in roleNames)
                {
                    if (role.NormalizedName!.Equals(rn.Trim().ToUpper()))
                    {
                        roles.Add(role);
                    }
                }
            }
            var userRoles = await _store.UserRoles.Where(ur => ur.Active && ur.UserId == user.Id).ToArrayAsync();
            return userRoles.Any(ur => roles.Any(r => r.Id == ur.RoleId));
        }

    }
}
