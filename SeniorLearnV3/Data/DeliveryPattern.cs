using AutoMapper.Execution;
using SeniorLearnV3.Data.Identity;
using static SeniorLearnV3.Areas.Member.Models.DeliveryPattern.Create;
using static SeniorLearnV3.Data.Enrollment;
using static SeniorLearnV3.Data.Lesson;

namespace SeniorLearnV3.Data
{
    public abstract class DeliveryPattern
    {
        protected DeliveryPattern()
        {

        }
        protected DeliveryPattern(string name, DateTime startOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
        {
            Name = name;
            StartOn = startOn;
            IsCourse = isCourse;
            DeliveryMode = deliveryMode;
        }

        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime StartOn { get; set; }
        public int ProfessionalId { get; set; }
        public Professional Professional { get; set; } = default!;
        public List<Lesson> Lessons { get; set; } = new();
        public bool IsCourse { get; set; }
        public Lesson.DeliveryModes DeliveryMode { get; set; }
        public enum DeliveryPatternTypes { NonRepeating = 0, Daily = 1, Weekly = 2 }
        public abstract DeliveryPatternTypes PatternType { get; }
        public abstract void GenerateLessons(Timetable timetable, string name, string description, int durationInMinutes, int capacity,Topic topic, string address, string url);

        public virtual List<Enrollment> EnrolMember(Member member, int lessonId)
        {
            //TODO: Demonstrate application of business rules and requirements being implemented in the model      
            if (!member.IsActiveStandardMember)
            {
                throw new ApplicationException("Only a Member in with a current active role of 'Standard Member' can enrol in a lesson");
            }
            if (member.ProfessionalRole == Professional)
            {
                throw new ApplicationException("A Professional cannot enrol in their own lesson (or related Professional Entity not loaded)");
            }
            var lesson = Lessons.First(l => l.Id == lessonId);

            if (lesson.Status != Lesson.LessonStatuses.Scheduled)
            {
                throw new ApplicationException("Enrolments can only be created for scheduled (open) lessons");
            }

            if (member.Enrollments.Any(e => e.Lesson == lesson))
            {
                throw new ApplicationException("A member can only enrol in a lesson once, existing enrolment detected");
            }
     
            if (lesson.EnrolledCount >= lesson.Capacity)
            {
                throw new ApplicationException("Sorry, Lesson is Closed. Capacity reach maximum, can't enrol!");
            }

            var result = new List<Enrollment>();

            if (!IsCourse)
            {
                var enrolment = new Enrollment(member, lesson, Enrollment.Statuses.Active);
                result.Add(enrolment);
                lesson.Enrollments.Add(enrolment);
                member.Enrollments.Add(enrolment);
                lesson.EnrolledCount++;
                if (lesson.EnrolledCount == lesson.Capacity)
                {
                    lesson.Status = Lesson.LessonStatuses.Closed; // Change lesson status to closed when capacity is max
                }
            }
            else
            {
                foreach (var l in Lessons)
                {
                    var enrolment = new Enrollment(member, l, Enrollment.Statuses.Active);
                    result.Add(enrolment);
                    l.Enrollments.Add(enrolment);
                    member.Enrollments.Add(enrolment);
                    l.EnrolledCount++;
                    if (l.EnrolledCount == lesson.Capacity) // when lesson capacity reach max, change lesson status to closed
                    {
                        l.Status = Lesson.LessonStatuses.Closed; // Change lesson status to closed when capacity is max
                    }

                }
            }
            return result;
        }

    }

    public class NonRepeating : DeliveryPattern
    {
        protected NonRepeating() { }
        public NonRepeating(string name, DateTime startOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
            : base(name, startOn, isCourse, deliveryMode) { }

        public override DeliveryPatternTypes PatternType => DeliveryPatternTypes.NonRepeating;

        public override void GenerateLessons(Timetable timetable, string name, string description, int durationInMinutes, int capacity, Topic topic, string address, string url)
        {
            timetable.ScheduleLesson(Professional, name, description, StartOn, durationInMinutes, capacity, this, topic, DeliveryMode, address, url);
        }
    }

    public abstract class Repeating : DeliveryPattern
    {
        protected Repeating() { }

        protected Repeating(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
            : base(name, startOn, isCourse, deliveryMode)
        {
            EndOn = endOn;
        }

        public DateTime EndOn { get; set; } = DateTime.MinValue; // End date for repeating pattern
    }

    public class Daily : Repeating
    {
        public Daily(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
           : base(name, startOn, endOn, isCourse, deliveryMode) { }

        public override DeliveryPatternTypes PatternType => DeliveryPatternTypes.Daily;

        public override void GenerateLessons(Timetable timetable, string name, string description, int durationInMinutes, int capacity, Topic topic, string address, string url)
        {
            var index = StartOn;
           
                int lessonCount = 1;
                while (index <= EndOn)
                {
                    timetable.ScheduleLesson(Professional, $"{(IsCourse ? lessonCount++ : "")} {name}", description, index, durationInMinutes, capacity, this, topic, DeliveryMode, address, url);
                    index = index.AddDays(1);
                }

        }
    }
    public class Weekly : Repeating
    {

        protected Weekly() { }

      //  protected Repeating(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
      //: base(name, startOn, isCourse, deliveryMode)
        public Weekly(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
          : base(name, startOn, endOn, isCourse, deliveryMode)
        {
        }

        public Weekly(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode, IList<DayOfWeek> days = default!)
         : base(name, startOn, endOn, isCourse, deliveryMode)
        {
            SetDays(days);
        }

        public void SetDays(IList<DayOfWeek> days)
        {
            Sunday = false; Monday = false; Tuesday = false; Wednesday = false; Thursday = false; Friday = false; Saturday = false;
            foreach (var d in days)
            {
                switch (d)
                {
                    case DayOfWeek.Sunday: Sunday = true; break;
                    case DayOfWeek.Monday: Monday = true; break;
                    case DayOfWeek.Tuesday: Tuesday = true; break;
                    case DayOfWeek.Wednesday: Wednesday = true; break;
                    case DayOfWeek.Thursday: Thursday = true; break;
                    case DayOfWeek.Friday: Friday = true; break;
                    case DayOfWeek.Saturday: Saturday = true; break;
                }
            }

        }
        private bool DoesDayMatch(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday: return Sunday;
                case DayOfWeek.Monday: return Monday;
                case DayOfWeek.Tuesday: return Tuesday;
                case DayOfWeek.Wednesday: return Wednesday;
                case DayOfWeek.Thursday: return Thursday;
                case DayOfWeek.Friday: return Friday;
                case DayOfWeek.Saturday: return Saturday;
                default: return false;
            }
        }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public override DeliveryPatternTypes PatternType => DeliveryPatternTypes.Weekly;



        public override void GenerateLessons(Timetable timetable, string name, string description, int durationInMinutes, int capacity, Topic topic, string address, string url)
        {
            var index = StartOn;
            int count = 0, lessonCount = 1;

            while (index <= EndOn)
            {
                if (DoesDayMatch(index.DayOfWeek))
                {
                    timetable.ScheduleLesson(Professional, $"{(IsCourse ? lessonCount++ : "")} {name}", description, index, durationInMinutes, capacity,this, topic, DeliveryMode, address, url);
                }
                index = index.AddDays(1);
                if (index.DayOfWeek == DayOfWeek.Sunday)
                {
                    count++;
                }
            }
        }
    }
}