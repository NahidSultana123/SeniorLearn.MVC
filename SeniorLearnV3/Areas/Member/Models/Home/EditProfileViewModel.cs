using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Areas.Member.Models.Home
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = default!;

        [Required]
        public string LastName { get; set; } = default!;
    }
}
