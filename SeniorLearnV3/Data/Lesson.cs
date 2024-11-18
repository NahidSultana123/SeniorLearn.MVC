using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data.Identity;
using SQLitePCL;

namespace SeniorLearnV3.Data
{
    public class Lesson
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Start { get; set; } // Individual lesson date and time
        public int DurationInMinutes { get; set; }
        public DateTime Finish => Start.AddMinutes(DurationInMinutes); //calculate the finish date and time    

        public int TimeTableId { get; set; }    // TODO: Need to work on this, use!
        public virtual Timetable Timetable { get; set; } = default!;
        public int DeliveryPatternId { get; set; }
        public DeliveryPattern DeliveryPattern { get; set; } = default!;
        public enum LessonStatuses { Draft = 1, Scheduled = 2, Closed = 3, Complete = 4, Cancelled = 5 }
        public LessonStatuses Status { get; set; } = LessonStatuses.Draft; //default status is Draft
        public enum DeliveryModes { OnPremises = 1, Online = 2 }
        public DeliveryModes DeliveryMode { get; set; }
        public string? Address { get; set; } // For OnPremises
        public string? URL { get; set; }   // For Online meetingn link or for onpremises googlemap link   

        public int Capacity { get; set; } // Maximum number of students
        public int EnrolledCount { get; set; } // Current number of enrolled students

        public int TopicId { get; set; }
        public Topic? Topic { get; set; } = default!; //Navigation property

        public virtual List<Enrollment> Enrollments { get; set; } = new();

        public List<Enrollment> Enrol(Member member) => DeliveryPattern.EnrolMember(member, Id);

        //public int ProfessionalId { get; set; }
        //public Professional Professional { get; set; } = default!; //Navigation property

        // Method to check if enrollment is possible
        public bool CanEnroll()
        {
            return Status == LessonStatuses.Scheduled && EnrolledCount < Capacity;
        }

        public void CreateLessonCancelNotification(ApplicationDbContext context, string title, string message, int fromMemberId)
        {
            
            var enrolmentList =  context.Enrollments
           .Where(e => e.LessonId == this.Id)
           .Include(e => e.Member) // Assuming Member is a navigation property in Enrollment
           .ToList();

            foreach(var e in enrolmentList)
            {
                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    FromMemberId = fromMemberId,
                    ToMemberId = e.Member.Id,
                    CreatedDate = DateTime.UtcNow,
                    NotificationType = Notification.NotificationTypes.LessonUpdate
                };

                context.Notifications.Add(notification);
            }
        }




    }
}
