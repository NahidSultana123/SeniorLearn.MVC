using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Policy;
using static SeniorLearnV3.Areas.Member.Models.DeliveryPattern.Create;

namespace SeniorLearnV3.Areas.Member.Models.DeliveryPattern
{
    public class LessonDetails
    {
        public LessonDetails(Data.Lesson lesson)
        {
            Id = lesson.Id;
            Name = lesson.Name;
            Description = lesson.Description;
            Start = lesson.Start.Date;
            StartTime = lesson.Start.ToString("hh:mm tt");
            FinishTime = (lesson.Start.AddMinutes(lesson.DurationInMinutes)).ToString("hh:mm tt");
            DurationInMinutes = lesson.DurationInMinutes;
            Status = lesson.Status.ToString(); // Assuming Status is an enum, convert to string           
            DeliveryMode = (DeliveryModes)lesson.DeliveryMode;
            Address = lesson.DeliveryMode == Data.Lesson.DeliveryModes.OnPremises ? lesson.Address : null;
            URL = lesson.DeliveryMode == Data.Lesson.DeliveryModes.Online ? lesson.URL : null;
            Capacity = lesson.Capacity;
            EnrolledCount = lesson.EnrolledCount;
            TopicId = lesson.TopicId;
            TopicName = lesson.Topic!.Name; // Assuming Topic is a navigation property
            DeliveryPatternId = lesson.DeliveryPatternId;
            DeliveryPatternName = lesson.DeliveryPattern.Name;
            IsCourse = lesson.DeliveryPattern.IsCourse; // Assuming this comes from DeliveryPattern
            //EndOn = lesson.DeliveryPattern is Repeating repeatingPattern ? repeatingPattern.EndOn : (DateTime?)null;
            ProfessionalId = lesson.DeliveryPattern.ProfessionalId;
            ProfessionalName = $"{lesson.DeliveryPattern!.Professional!.User!.Member!.FirstName} {lesson.DeliveryPattern.Professional.User.Member.LastName}";
            DeliveryPatternType = (DeliveryPatternTypes)lesson.DeliveryPattern.PatternType;
            if (lesson.DeliveryPattern.PatternType is Data.DeliveryPattern.DeliveryPatternTypes.Weekly)
            {

            }
            if (lesson.DeliveryPattern is Repeating repeatingPattern)
            {
                EndOn = repeatingPattern.EndOn;
                if (repeatingPattern is Weekly weeklyPattern)
                {
                    WeeklyDays = new List<string>();
                    AddWeekDays(weeklyPattern); // Generate the days for weekly pattern
                }
            }
        }

            public int Id { get; set; }
             
            [Required]
            public string? Name { get; set; } = default!;
            public string? Description { get; set; }

            //[Display(Name = "Start")]
             public DateTime Start { get; set; }

             public string? StartTime { get; set; }

             public string? FinishTime{ get; set; }

            [Display(Name = "Duration in Minutes ")]
            public int DurationInMinutes { get; set; } // Duration of the lesson

            //public DateTime Finish => Start.AddMinutes(DurationInMinutes);

            public string Status { get; set; } = default!; // Lesson status as string (e.g., "Draft", "Scheduled", etc.)

            // public string DeliveryMode { get; set; } = default!; // Delivery mode as string (e.g., "Online", "OnPremises")

            public enum DeliveryModes { OnPremises = 1, Online = 2 }
            public DeliveryModes DeliveryMode { get; set; }

            public enum DeliveryPatternTypes { NonRepeating = 0, Daily = 1, Weekly = 2 }
            public DeliveryPatternTypes DeliveryPatternType { get; set; }

            public List<string> WeeklyDays { get; set; } = new();
            private void AddWeekDays(Weekly weeklyPattern)
            {
                if (weeklyPattern.Sunday) WeeklyDays.Add("Sunday");
                if (weeklyPattern.Monday) WeeklyDays.Add("Monday");
                if (weeklyPattern.Tuesday) WeeklyDays.Add("Tuesday");
                if (weeklyPattern.Wednesday) WeeklyDays.Add("Wednesday");
                if (weeklyPattern.Thursday) WeeklyDays.Add("Thursday");
                if (weeklyPattern.Friday) WeeklyDays.Add("Friday");
                if (weeklyPattern.Saturday) WeeklyDays.Add("Saturday");
             }


        public string? Address { get; set; } // Optional: Address if OnPremises

            public string? URL { get; set; } // Optional: URL if Online or a map link for OnPremises

            [Display(Name = "Maximun Number of Students")]
            public int Capacity { get; set; } // Maximum capacity for enrollment

            [Display(Name = "Total Number of Students Enroled")]
            public int EnrolledCount { get; set; } // Number of enrolled students
            public int TopicId { get; set; }

            [Display(Name = "Topic")]
            public string TopicName { get; set; } = default!; // The name of the associated topic
            public int DeliveryPatternId { get; set; }
            public SeniorLearnV3.Data.DeliveryPattern DeliveryPattern { get; set; } = default!;
            public string DeliveryPatternName { get; set; } = default!;

            [Display(Name = "Is Course?")]
            public bool IsCourse { get; set; }

            [Display(Name = "End On")] // To work on future
            public DateTime? EndOn { get; set; }

            public int ProfessionalId { get; set; }
            public Professional Professional { get; set; } = new();

            [Display(Name = "Offered By")]
            public string ProfessionalName { get; set; } = default!; // The name of the professional who created the lesson


        }
}
