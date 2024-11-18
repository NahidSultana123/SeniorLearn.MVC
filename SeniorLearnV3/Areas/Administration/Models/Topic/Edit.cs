using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Areas.Administration.Models.Topic
{
    public class Edit
    {
        public int Id { get; set; } 

        [Required]
        public string? Name { get; set; } = default!;

        [Required]
        public string? Description { get; set; } = default!;
    }
}
