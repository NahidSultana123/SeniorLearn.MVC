namespace SeniorLearnV3.Data
{
    public class Enrollment
    {
        private Enrollment()
        {

        }

        public Enrollment(Member member, Lesson lesson, Statuses status = Statuses.Active)
        {
            Member = member;
            Lesson = lesson;
            Status = status;
            EnrollmentDate = DateTime.Now;
        }

        public int Id { get; set; } // Primary Key
        public int LessonId { get; set; } // Foreign Key to Lesson
        public int MemberId { get; set; } // Foreign Key to Member
        public DateTime EnrollmentDate { get; set; } // Date of enrollment
        public Lesson Lesson { get; set; } = default!; // Navigation property
        public Member Member { get; set; } = default!; // Navigation property
        public enum Statuses { Active = 1, Participated = 2, Withdrawn = 3, NoShow = 4 }
        public Statuses Status { get; set; } = Statuses.Active;
        

    }
}
