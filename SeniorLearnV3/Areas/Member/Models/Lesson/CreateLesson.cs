//using Microsoft.AspNetCore.Mvc.Rendering;
//using SeniorLearnV3.Data;
//using System.ComponentModel.DataAnnotations;
//using static SeniorLearnV3.Data.Lesson;


//namespace SeniorLearnV3.Areas.Member.Models.Lesson
//{
//    public class CreateLesson
//    {
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
//          => new SelectList(new[]{
//                new { Text = "Draft", Value = 1 },
//                new { Text = "Scheduled", Value = 2 }
//          }, "Value", "Text");
       

//        //public SelectList LessonStatuses { get; set; } = new SelectList(Enum.GetValues<LessonStatuses>().Cast<LessonStatuses>()
//        //  .Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text");


//        public int DeliveryModeId { get; set; }
//        public SelectList DeliveryModes
//         => new SelectList(new[]{
//                new { Text = "OnPremises", Value = 1 },
//                new { Text = "Online", Value = 2 }
//         }, "Value", "Text");

//        //public SelectList DeliveryModes { get; set; } = new SelectList(Enum.GetValues<DeliveryModes>().Cast<DeliveryModes>()
//        //  .Select(d => new { Value = (int)d, Text = d.ToString() }), "Value", "Text");

//        // Address is optional and relevant only for OnPremises mode
//        public string? Address { get; set; }

//        // URL is optional and relevant only for Online mode (or Google Maps link)
//        public string? URL { get; set; }

//        [Required]
//        public int Capacity { get; set; } // Maximum number of students

        
//        public int TopicId { get; set; }
//        public SelectList? Topics { get; set; } = default!; // Populated from the controller

//        public int ProfessionalId { get; set; } // Professional creating the lesson


//    }
//}
