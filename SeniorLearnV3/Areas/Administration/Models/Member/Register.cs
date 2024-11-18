using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Areas.Administration.Models.Member
{
    //TODO: Data Annotations (more customization/validation required)
    public class Register
    {
        [Display(Name = "Given Name")]
        public string FirstName { get; set; } = default!; 
        [Display(Name = "Surname")]
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}