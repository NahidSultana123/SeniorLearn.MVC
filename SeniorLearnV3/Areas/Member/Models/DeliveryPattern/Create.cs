using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data;
using System.ComponentModel.DataAnnotations;
using static SeniorLearnV3.Data.DeliveryPattern;

namespace SeniorLearnV3.Areas.Member.Models.DeliveryPattern
{
    public class Create
    {
        public Create()
        {
                
        }

        public Create(ApplicationDbContext context, int organisationId, string lessonName = "", string lessonDescription = "", int durationInMinutes = 60, int capacity = 0)
        {
            InitTemplate(context, organisationId, lessonName, lessonDescription, durationInMinutes, capacity).Wait();
        }

        public string Name { get; set; } = default!;
        public DateTime StartOn { get; set; }
        public bool IsCourse { get; set; }

        public int ProfessionalId { get; set; } // Professional creating the lesson
                                                // Lesson-related properties (template)
        public Lesson? Template { get; set; }

        // TODO: Check if needed, it belongs to Lesson class
        //public int DeliveryModeId { get; set; }
        //public SelectList DeliveryModes
        // => new SelectList(new[]{
        //        new { Text = "OnPremises", Value = 1 },
        //        new { Text = "Online", Value = 2 }
        // }, "Value", "Text");

        public int PatternType { get; set; }
        public SelectList DeliveryPatternTypes
         => new SelectList(new[]{
                new { Text = "Non Repeating", Value = 1 },
                new { Text = "Daily", Value = 2 },
                new{Text ="Weekly", Value = 3}
                            }, "Value", "Text");

        public int DeliveryModeId { get; set; }
        public SelectList DeliveryModes
         => new SelectList(new[]{
                new { Text = "OnPremises", Value = 1 },
                new { Text = "Online", Value = 2 }
         }, "Value", "Text");

        public string? Address { get; set; } = ""; // For OnPremises delivery
        public string? URL { get; set; } = ""; // For Online delivery

        [DataType(DataType.Date)]
        public DateTime EndOn { get; set; } = DateTime.MinValue;
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }

        public DeliveryPatternTypes DeliveryPattern => (DeliveryPatternTypes)PatternType;
        public List<DayOfWeek> Days
        {
            get
            {
                var days = new List<DayOfWeek>();
                if (Sunday) { days.Add(DayOfWeek.Sunday); }
                if (Monday) { days.Add(DayOfWeek.Monday); }
                if (Tuesday) { days.Add(DayOfWeek.Tuesday); }
                if (Wednesday) { days.Add(DayOfWeek.Wednesday); }
                if (Thursday) { days.Add(DayOfWeek.Thursday); }
                if (Friday) { days.Add(DayOfWeek.Friday); }
                if (Saturday) { days.Add(DayOfWeek.Saturday); }
                return days;
            }
        }
        // Lesson properties

        public class Lesson
        {
            public string Name { get; set; } = default!;
            public string Description { get; set; } = default!;
            //public DateTime LessonStart { get; set; } // Date and time for the first lesson
            public int DurationInMinutes { get; set; }
            public int Capacity { get; set; }
            public int TopicId { get; set; }
            public SelectList? Topics { get; set; } = default!; // Populated from the controller
        }

        public async Task InitTemplate(ApplicationDbContext context, int organisationId, string lessonName = "", string lessonDescription = "", int durationInMinutes = 60, int capacity = 0)
        {
            Template = new Lesson
            {
                Name = lessonName,
                Description = lessonDescription,
                DurationInMinutes = durationInMinutes,
                Capacity = capacity,
                Topics = await GetTopicSelectList(context, organisationId)
            };
        }
        //private async Task<SelectList> GetTopicSelectList(ApplicationDbContext c, int oId)
        //   => new SelectList(await c.Topics.Where(t => t.OrganisationId == oId).ToArrayAsync(), nameof(Topic.Id), nameof(Topic.Name));
        //------------------------------------OR----------------------------------------------
        private async Task<SelectList> GetTopicSelectList(ApplicationDbContext context, int organisationId)
        {
            var topics = await context.Topics
                                      .Where(t => t.OrganisationId == organisationId)
                                      .ToArrayAsync();

            return new SelectList(topics, nameof(Topic.Id), nameof(Topic.Name));
        }

        public async Task RebuildTopics(ApplicationDbContext context, int organisationId)
        {
            Template!.Topics = await GetTopicSelectList(context, organisationId);
        }

    }
}
