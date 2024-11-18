using static SeniorLearnV3.Data.Notification;

namespace SeniorLearnV3.Data
{
    public class Notification
    {
        public int Id { get; set; }
        public int FromMemberId { get; set; } // Link to the sender
        public int ToMemberId { get; set; } // Link to the recipient
        public virtual Member FromMember { get; set; } = default!; // Sender navigation property
        public virtual Member ToMember { get; set; } = default!;
        public string Title { get; set; } = default!; // Notification title, e.g., "Lesson Cancelled"
        public string Message { get; set; } = default!; // Notification body
        public DateTime CreatedDate { get; set; } = DateTime.Now; // When notification was created
        public bool IsRead { get; set; } = false; // Mark if the member has read it

        public NotificationTypes NotificationType { get; set; } = NotificationTypes.General;

        // Enum for notification types
        public enum NotificationTypes  //various types for future extension
        {
            General = 1, // General updates or announcements
            LessonUpdate = 2, // Lesson changes- cancellation or rescheduling or any [Will work on this first]
            EnrollmentConfirmation = 3, // Confirmation of enrollment actions
            MembershipExpiration = 4, // Membership going to expire
            SystemUpdate = 5 // e.g. - System maintenance
        }
    }
}
