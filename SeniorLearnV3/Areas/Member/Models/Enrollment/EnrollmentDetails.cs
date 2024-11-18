using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Areas.Member.Models.Enrollment
{
    public class EnrollmentDetails
    {
        public int EnrollmentId { get; set; }
        public int LessonId { get; set; }

        [Display(Name ="Lesson Name")]
        public string? LessonName { get; set; } 
        public int AvailableSpace { get; set; } // calculate how many seats available
        public int EnrolledCount { get; set; }
        public DateTime LessonStartDate { get; set; }
        public string? MemberName { get; set; } 
        public string? Status { get; set; } 
    }
}
