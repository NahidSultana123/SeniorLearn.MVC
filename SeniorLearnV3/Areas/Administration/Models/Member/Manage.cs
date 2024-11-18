using SeniorLearnV3.Data.Identity;

namespace SeniorLearnV3.Areas.Administration.Models.Member
{
    //TODO: Use View Models (done)
    public class Manage : Register
    {
        public int Id { get; set; }
        public DateTime RenewalDate { get; set; }
        public decimal OutstandingFees { get; set; }
        public List<UserRole> Roles { get; set; } = new();
        public bool IsActiveStandardMember { get;  set; }
        public bool IsActiveProfessionalMember { get;  set; }
        public bool IsActiveHonoraryMember { get; set; }
    }
}
