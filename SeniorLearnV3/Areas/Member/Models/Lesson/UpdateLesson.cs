//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.ComponentModel.DataAnnotations;

//namespace SeniorLearnV3.Areas.Member.Models.Lesson
//{
//    public class UpdateLesson
//    {
//        public int Id { get; set; } // The Id of the lesson being updated

//        [Required]
//        public string? Name { get; set; }

//        [Required]
//        public string? Description { get; set; }

//        [Required]
//        [DataType(DataType.DateTime)]
//        public DateTime StartDate { get; set; } = DateTime.Now;

//        [Required]
//        public int DurationInMinutes { get; set; }

//        public int LessonStatusId { get; set; }
//        public SelectList LessonStatuses
//            => new SelectList(new[] {
//                new { Text = "Draft", Value = 1 },
//                new { Text = "Scheduled", Value = 2 },
//                new { Text = "Closed", Value = 3 },
//                new { Text = "Complete", Value = 4 },
//                new { Text = "Cancelled", Value = 5 }
//            }, "Value", "Text");

//        public int DeliveryModeId { get; set; }
//        public SelectList DeliveryModes
//            => new SelectList(new[] {
//                new { Text = "OnPremises", Value = 1 },
//                new { Text = "Online", Value = 2 }
//            }, "Value", "Text");

//        // Optional properties for OnPremises and Online
//        public string? Address { get; set; }
//        public string? URL { get; set; }

//        [Required]
//        public int Capacity { get; set; } // Maximum number of students

//        public int TopicId { get; set; }
//        public SelectList? Topics { get; set; } = default!; // Populated from the controller

//        //public int ProfessionalId { get; set; } // Professional updating the lesson
//    }
//}
