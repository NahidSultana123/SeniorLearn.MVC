using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Areas.Member.Controllers;
using SeniorLearnV3.Data;

namespace SeniorLearnV3.Areas.Member.Controllers
{
   
    public class CalendarController : MemberAreaController
    {
        public CalendarController(ApplicationDbContext context):base(context) { }

        // Fetches a list of lessons asynchronously from the database
        // Returns the view, ensuring that the lesson list is checked for null or empty before processing further

        public async Task<IActionResult> Index()
        {
            var lessonlist = await _context.Lessons.ToListAsync();
         
            if (lessonlist == null || lessonlist.Count == 0)
            {
                return NotFound("No lessons available to display.");
            }
            return View();
        }


        // Fetches a list of lessons from the database and formats them for FullCalendar
        // Includes related data like DeliveryPattern, Professional, and Topic using eager loading
        // Filters out lessons in Draft status and maps data into a JSON format suitable for FullCalendar
        // Provides additional details (e.g., lesson details, start time, delivery mode) as extended properties
        // Returns the JSON result to be used by the FullCalendar instance
        [HttpGet] 
        public async Task<JsonResult> GetLessons()
        {
            
            var lessonslist = await _context.Lessons
                .Include(l => l.DeliveryPattern)
                    .ThenInclude(l => l.Professional)
                    .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Member)
                .Include(l => l.Topic) //Include Topic data
                .Where(l => l.Status != Lesson.LessonStatuses.Draft) // Fetch lessons that is not in Draft status
                .Select(l => new
                {
                    id = l.Id,
                    title = l.Topic!.Name, //Instead of Lesson name, I am displaying topic name in calendar
                    //title = l.Name,
                    start = l.Start.ToString("s"), // ISO format for FullCalendar                                             
                    description = l.Description,
                    url = l.URL,
                    allDay = false, // Set allDay false to show specific time
                    extendedProps = new
                    {
                        // TODO: Create limited description here - not working find out later why?
                        // limitedDescription = string.Join(" ", l.Description!.Split(' ').Take(10)) + (l.Description.Split(' ').Length > 10 ? "..." : ""),
                        //ToString("dd/MM/yyyy h:mm tt")
                        lessonId = l.Id,
                        lessonDetails = $"<strong>Lesson Name: {l.Name}</strong>" +
                        $"<br>Start: {l.Start.ToString("h:mm tt")}" +
                        $" End: {l.Start.AddMinutes(l.DurationInMinutes).ToString("h:mm tt")}" + 
                        $"<br>Course:{l.DeliveryPattern.IsCourse}" +
                        $"<br>Delivery Mode: {l.DeliveryMode}" +
                        $"<br> Created By: {l.DeliveryPattern!.Professional!.User!.Member!.FirstName} {l.DeliveryPattern.Professional.User.Member.LastName}"
                    }

                })
                .ToListAsync();

            return Json(lessonslist);
        }


    }
}
