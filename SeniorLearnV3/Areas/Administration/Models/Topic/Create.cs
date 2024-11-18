
using SeniorLearnV3.Data;
using System.ComponentModel.DataAnnotations;


namespace SeniorLearnV3.Areas.Administration.Models.Topic
{
    public class Create
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;
        public int OrganisationId { get; set; }
      
    }
}

