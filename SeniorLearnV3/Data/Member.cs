using SeniorLearnV3.Data.Identity;
using System.Data;
using static SeniorLearnV3.Data.Identity.UserRole;

namespace SeniorLearnV3.Data
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        //this is subject to updates as a response to  role update events 
        public DateTime RenewalDate { get; set; } = default!;
        public decimal OutstandingFees { get; set; }
        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; } = default!;
        public byte[] RowVersion { get; set; } = default!;
        // public virtual List<Enrolment> Enrolments { get; set; } = new();  // TODO: Add later 
        public virtual List<Payment> Payments { get; set; } = new();
        public virtual List<Enrollment> Enrollments { get; set; } = new();
        public string? UserId { get; set; }
        public virtual User User { get; set; } = default!;


        //public virtual IReadOnlyList<UserRole> Roles => User?.Roles!; // TODO: If needed go back
        // Safely access Roles
        public virtual IReadOnlyList<UserRole> Roles => User?.Roles ?? new List<UserRole>();

        public Professional ProfessionalRole => Roles.OfType<Professional>().FirstOrDefault()!;

        public bool IsInActiveRole(UserRole.RoleTypes roleType) => User.IsInActiveRole(roleType);
        //public bool IsInActiveRole(UserRole.RoleTypes roleType) => User?.IsInActiveRole(roleType) ?? false;
        public bool IsActiveStandardMember => IsInActiveRole(UserRole.RoleTypes.STANDARD);
        public bool IsActiveProfessionalMember => IsInActiveRole(UserRole.RoleTypes.PROFESSIONAL);
        public bool IsActiveHonoraryMember => IsInActiveRole(UserRole.RoleTypes.HONORARY);

        public UserRole UpdateStandardRole(bool active, string notes = "") => User.UpdateStandardRole(active, notes);
        public UserRole UpdateProfessionalRole(string discipline, bool active = true, string notes = "") => User.UpdateProfessionalRole(discipline, active, notes);
        public UserRole UpdateHonoraryRole(bool active = true, string notes = "") => User.UpdateHonoraryRole(active, notes);

        // Navigation property for notifications
        public virtual List<Notification> SentNotifications { get; set; } = new(); // Notifications sent by this member
        public virtual List<Notification> ReceivedNotifications { get; set; } = new(); // Notifications received by this member
        public decimal CalculateOutstandingFees(string highestActiveRoleId, Organisation organisation)
        {

            decimal totalPaid=0.0m;

            // Determine the membership fee based on the member's role
            decimal membershipFee = 0.0m;

            // Determine the membership fee based on the active role
            if(highestActiveRoleId=="STANDARD")
            {
                totalPaid = Payments.Where(p => p.Approved).Sum(p => p.Amount);
                membershipFee = organisation.StandardMembershipFee;
            }
            else if(highestActiveRoleId == "PROFESSIONAL") 
            {
                membershipFee = organisation.ProfessionalMembershipFee;
            }
            else if (highestActiveRoleId == "HONORARY")
            {
                membershipFee = organisation.HonoraryMembershipFee;
            }
            else
            {
                membershipFee = 0;
            }

             //membershipFee = IsActiveStandardMember ? Organisation.StandardMembershipFee :
                                //IsActiveProfessionalMember ? Organisation.ProfessionalMembershipFee :
                               // IsActiveHonoraryMember ? Organisation.HonoraryMembershipFee : 0;

            // Calculate outstanding fees
            var outstandingFees = membershipFee - totalPaid;

            // To ensure that outstanding fees is not negative
            return outstandingFees > 0 ? outstandingFees : 0;
        }
    }

  
}
