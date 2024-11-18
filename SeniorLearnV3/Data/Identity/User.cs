using Microsoft.AspNetCore.Identity;

namespace SeniorLearnV3.Data.Identity
{
    public class User : IdentityUser
    {
        public virtual Member? Member { get; set; }
        public virtual List<UserRole> Roles { get; set; } = new();

        // TODO: extend later

        // Check if the user is in the Admin role
        public bool IsInAdminRole => Roles.Any(r => r.Role.Name == "ADMINISTRATION");

        // Check if the user has an active role of the specified type
        public bool IsInActiveRole(UserRole.RoleTypes roleType) => Roles.Any(r => r.RoleType == roleType && r.Active);

        public UserRole UpdateStandardRole(bool active = true, string notes = "")
        {
            if (Member == null)
            {
                throw new ApplicationException("No Associated Member Object, User is either not a member or associated entity is not loaded");
            }
            var role = Roles.OfType<Standard>().FirstOrDefault();

            if (role == null)
            {
                role = new Standard
                {
                    Active = active,
                    RegistrationDate = DateTime.Now,
                    RoleId = UserRole.RoleTypes.STANDARD.ToString()
                };
                Roles.Add(role);
            }
            else
            {
                role.Active = active;
            }

            var update = new RoleUpdate
            {
                Active = role.Active,
                Timestamp = DateTime.Now,
                RenewaDate = Member.RenewalDate,
                Notes = $"{role.RoleType}: {notes.Trim().ToUpper()}"
            };

            role.Updates.Add(update);
            return role;
        }

        public UserRole UpdateProfessionalRole(string discipline, bool active = true, string notes = "")
        {

            if (Member == null)
            {
                throw new ApplicationException("No Associate Member Object, User is either not a member or associated entity is not loaded");
            }
            var role = Roles.OfType<Professional>().FirstOrDefault();

            if (role == null)
            {
                role = new Professional
                {
                    Active = active,
                    Discipline = discipline,
                    RoleId = UserRole.RoleTypes.PROFESSIONAL.ToString(),
                };
                Roles.Add(role);
            }
            else
            {
                role.Active = active;
                role.Discipline = discipline;
            }

            var update = new RoleUpdate
            {
                Active = role.Active,
                Timestamp = DateTime.Now,
                RenewaDate = Member.RenewalDate,
                Notes = $"{role.RoleType}: {notes.Trim().ToUpper()}"
            };

            role.Updates.Add(update);
            return role;
        }

        public UserRole UpdateHonoraryRole(bool active = true, string notes = "")
        {
            if (Member == null)
            {
                throw new ApplicationException("No Associate Member Object, User is either not a member or associated entity is not loaded");
            }
            var role = Roles.OfType<Honorary>().FirstOrDefault();


            if (role == null)
            {
                role = new Honorary
                {
                    Active = active,
                    RoleId = UserRole.RoleTypes.HONORARY.ToString(),
                    Notes = ""
                };
                Roles.Add(role);
            }
            else
            {
                role.Active = active;
            }

            var update = new RoleUpdate
            {
                Active = role.Active,
                Timestamp = DateTime.Now,
                RenewaDate = Member.RenewalDate,
                Notes = $"{role.RoleType}: {notes.Trim().ToUpper()}"
            };

            role.Updates.Add(update);
            return role;
        }

    }
}
