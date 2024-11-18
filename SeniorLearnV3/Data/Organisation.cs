using SeniorLearnV3.Extensions;
using System.Xml;
using SeniorLearnV3.Data.Identity;

namespace SeniorLearnV3.Data
{
    public class Organisation
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public virtual List<Member> Members { get; set; } = new();

        public int TimetableId { get; set; }
        public virtual Timetable Timetable { get; set; } = default!;

        public List<Topic> Topics { get; set; } = new();

        public decimal StandardMembershipFee { get; set; } = 50.0m; // Standard fee
        public decimal ProfessionalMembershipFee { get; set; } = 0.0m; // Professional is exempt while active
        public decimal HonoraryMembershipFee { get; set; } = 0.0m; // Honorary is exempt


        public Member RegisterMember(Identity.User user, string firstName, string lastName, string email)
        {
            var member = new Member { FirstName = firstName, LastName = lastName, Email = email, User = user };
            member.RenewalDate = DateTime.Now.GetNextMonthlyRenewalDate();
            //member.OutstandingFees = 0.0m;
            member.OutstandingFees = 50.0m; //m stands for decimal number else it will assume double
            
            user.Member = member; // Associate user with member
            //member.IsActiveStandardMember = true;
            member.UpdateStandardRole(active: true, notes: "Initial Registration");
            Members.Add(member);
            return member;
        }
    }
}
