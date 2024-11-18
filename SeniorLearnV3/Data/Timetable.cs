using SeniorLearnV3.Data.Identity;

namespace SeniorLearnV3.Data
{
    public class Timetable
    {

        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; } = default!;

        // TODO: Add lesson later - Done- purpose?

        public virtual List<Lesson> Lessons { get; set; } = new();

        public Lesson ScheduleLesson
         (
        Professional professional,
        string name,
        string description,
        DateTime start,
        int durationInMinutes,
        int capacity,
        DeliveryPattern deliveryPattern,
        Topic topic,
        Lesson.DeliveryModes deliveryMode,
        string address="" ,
        string url = ""
         )
        {
            Lesson lesson = new Lesson
            {
                Name = name,
                Description = description,
                Start = start,
                DurationInMinutes = durationInMinutes,
                Capacity = capacity,
                EnrolledCount = 0,
                Status = Lesson.LessonStatuses.Draft,
                Topic = topic,
                DeliveryMode = deliveryMode,
                Address = deliveryMode == Lesson.DeliveryModes.OnPremises ? address : null,
                URL = deliveryMode == Lesson.DeliveryModes.Online ? url : null,
                //Professional = professional
            };

            Lessons.Add(lesson);
            deliveryPattern.Lessons.Add(lesson);
            return lesson;
        }


    }


}
