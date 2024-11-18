using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Areas.Member.Models.Lesson
{
    public class LessonDetails
    {

        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Duration")]
        public int DurationInMinutes { get; set; } // Duration of the lesson
        public string Status { get; set; } = default!; // Lesson status as string (e.g., "Draft", "Scheduled", etc.)

        public string DeliveryMode { get; set; } = default!; // Delivery mode as string (e.g., "Online", "OnPremises")

        public string? Address { get; set; } // Optional: Address if OnPremises

        public string? URL { get; set; } // Optional: URL if Online or a map link for OnPremises

        public int Capacity { get; set; } // Maximum capacity for enrollment

        public int EnrolledCount { get; set; } // Number of enrolled students

        [Display(Name = "Topic")]
        public string TopicName { get; set; } = default!; // The name of the associated topic

        [Display(Name = "Offered By")]
        public string ProfessionalName { get; set; } = default!; // The name of the professional who created the lesson
    }
}
