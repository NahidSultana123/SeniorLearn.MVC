using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SeniorLearnV3.Areas.Member.Controllers;
using SeniorLearnV3.Areas.Member.Models.DeliveryPattern;
using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;
using static SeniorLearnV3.Data.Lesson;


namespace SeniorLearnV3.Areas.Member.Controllers
{
    public class DeliveryPatternController : MemberAreaController
    {
        public DeliveryPatternController(ApplicationDbContext context) : base(context) { }

        //To display list of delivery plans for the user
        public async Task<IActionResult> Index()
        {
            var member = await _context.FindMemberAsync(User);

            var patterns = await _context.DeliveryPatterns
                    .Include(p => p.Lessons)
                    .Where(p => p.ProfessionalId == member.ProfessionalRole.Id)
                    .ToListAsync();

            return View(patterns);
        }

        //Get Template to create new Delivery Plan Including Lesson Info
        public async Task<IActionResult> Create()
        {
            var member = await _context.FindMemberAsync(User);
            var m = new Models.DeliveryPattern.Create(_context, member!.OrganisationId)
            {
                PatternType = 0,//0=None,1=Daily,2=Weekly
                //EndStrategyId = 1,//1=Occurrence,2=Date    // TODO: I don't have EndStrategy options in my code
                //Occurrences = 1,                 // TODO: Don't have Occurences option, If needed will add later
                StartOn = DateTime.Now,
                EndOn = DateTime.Now.AddMonths(1),
                //Initialize = true  // TODO: I don't have Initialize property in Delivery Pattern
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.DeliveryPattern.Create m)
        {
            var member = await _context.FindMemberAsync(User);
            if (ModelState.IsValid)
            {
                var role = member!.Roles.OfType<Professional>().First();

                var mode = (Lesson.DeliveryModes)m.DeliveryModeId;


                var deliveryPattern = role.AddDeliveryPattern(m.Name, m.StartOn, m.DeliveryPattern, m.IsCourse, mode, m.EndOn, m.Days);

                var template = m.Template;
                var topic = await _context.Topics.FirstAsync(t => t.Id == template!.TopicId);
                deliveryPattern.GenerateLessons(member.Organisation.Timetable, template!.Name, template.Description, template.DurationInMinutes, template.Capacity, topic, m.Address, m.URL);


                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            await m.RebuildTopics(_context, member!.OrganisationId);
            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> Lessons(int id)
        {
            var pattern = await _context.DeliveryPatterns
                .Include(dp => dp.Lessons).ThenInclude(l => l.Topic)
                .FirstAsync(dp => dp.Id == id);
            return View(pattern);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateLesson(int id)
        {


            // var lesson = await _context.Lessons.Include(l => l.Topic) // Include topic information if needed
            // .FirstOrDefaultAsync(l => l.Id == id);

            var lesson = await _context.Lessons.Include(l => l.Topic).FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            var topics = await _context.Topics.ToListAsync();

            var model = new Models.DeliveryPattern.UpdateLesson
            {
                Id = lesson.Id,
                Name = lesson.Name,
                Description = lesson.Description,
                Start = lesson.Start,
                DurationInMinutes = lesson.DurationInMinutes,
                LessonStatusId = (int)lesson.Status, // Assuming Status is an enum
                DeliveryModeId = (int)lesson.DeliveryMode, // Assuming DeliveryMode is an enum
                Address = lesson.Address,
                URL = lesson.URL,
                Capacity = lesson.Capacity,
                TopicId = lesson.TopicId,
                Topics = new SelectList(topics, "Id", "Name"), // Populate the topics
                                                               //ProfessionalId = lesson.ProfessionalId // Professional who created the lesson
            };

            return View(model);
        }

        // Method to update single lesson if not course,  multiple lessons if course
        [HttpPost]
        public async Task<IActionResult> UpdateLesson(UpdateLesson model)
        {
            if (ModelState.IsValid)
            {
                //var lesson = await _context.Lessons.FindAsync(model.Id);
                var lesson = await _context.Lessons
                               .Include(l => l.Timetable)
                               .Include(l => l.DeliveryPattern)
                               .FirstAsync(l => l.Id == model.Id);

                var deliveryPattern = await _context.DeliveryPatterns.FirstAsync(dp => dp.Id == lesson.DeliveryPatternId);


                if (lesson == null)
                {
                    return NotFound();
                }

                if (lesson.DeliveryPattern.IsCourse == false)
                {
                    //// Update lesson properties
                    lesson.Name = model.Name;
                    lesson.Description = model.Description;
                    lesson.Start = model.Start;
                    lesson.DurationInMinutes = model.DurationInMinutes;
                    lesson.Status = (Lesson.LessonStatuses)model.LessonStatusId; // Map the selected status
                    lesson.DeliveryMode = (Lesson.DeliveryModes)model.DeliveryModeId; // Map the selected delivery mode
                    lesson.Address = model.Address;
                    lesson.URL = model.URL;
                    lesson.Capacity = model.Capacity;
                    lesson.TopicId = model.TopicId; // Assign selected topic


                }
                else if (lesson.DeliveryPattern.IsCourse == true)
                {
                    TempData["Message"] = "This lesson belongs to a course. Changing lesson's deliverymode/capacity will effect all corresponding lessons.";

                    deliveryPattern.DeliveryMode = (Lesson.DeliveryModes)model.DeliveryModeId;


                    //// Update lesson properties
                    lesson.Name = model.Name;
                    lesson.Description = model.Description;
                    lesson.Start = model.Start;

                    //lesson.DeliveryMode = (Lesson.DeliveryModes)model.DeliveryModeId; // Map the selected delivery mode
                    //lesson.Address = model.Address;
                    //lesson.URL = model.URL;
                    //lesson.Capacity = model.Capacity;

                    var lessonsInCourse = _context.Lessons.Where(l => l.DeliveryPatternId == lesson.DeliveryPatternId).ToList();


                    //if (lesson.DeliveryMode == Lesson.DeliveryModes.OnPremises)
                    if ((Lesson.DeliveryModes)model.DeliveryModeId == Lesson.DeliveryModes.OnPremises)
                    {
                        foreach (var l in lessonsInCourse)
                        {
                            l.DurationInMinutes = model.DurationInMinutes;
                            l.DeliveryMode = (Lesson.DeliveryModes)model.DeliveryModeId;
                            l.Address = model.Address; // Update Address if OnPremises
                            l.URL = null; // Set URL to null
                            l.Capacity = model.Capacity;
                            l.TopicId = model.TopicId; // Assign selected topic
                            l.Status = (Lesson.LessonStatuses)model.LessonStatusId; // Map the selected status
                        }


                    }
                    //else if(lesson.DeliveryMode == Lesson.DeliveryModes.Online)
                    else if ((Lesson.DeliveryModes)model.DeliveryModeId == Lesson.DeliveryModes.Online)
                    {
                        foreach (var l in lessonsInCourse)
                        {
                            l.DurationInMinutes = model.DurationInMinutes;
                            l.DeliveryMode = (Lesson.DeliveryModes)model.DeliveryModeId;
                            l.Address = null; // Update Address if OnPremises
                            l.URL = model.URL; // Set URL to null
                            l.Capacity = model.Capacity;
                            l.TopicId = model.TopicId; // Assign selected topic
                            l.Status = (Lesson.LessonStatuses)model.LessonStatusId; // Map the selected status
                        }
                    }


                }

                await _context.SaveChangesAsync();
                //return RedirectToAction("Lessons");
                return RedirectToAction("Index"); // Redirect to the lesson list or details page
            }

            // Repopulate topics if model state is invalid
            model.Topics = new SelectList(await _context.Topics.ToListAsync(), "Id", "Name");

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CancelLesson(int id, string title, string message)
        {
            var member = await _context.FindMemberAsync(User);

            var lesson = await _context.Lessons.
                Include(l => l.DeliveryPattern)
                .Include(l => l.Enrollments) // Load enrollments if necessary
                .ThenInclude(e => e.Member) // Load members if necessary
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound();
            }
            if (!lesson.DeliveryPattern.IsCourse)
            {
                // Change the lesson status to Cancelled
                lesson.Status = Lesson.LessonStatuses.Cancelled;
            }
            else
            {
                var lessonList = await _context.Lessons
                  .Where(l => l.DeliveryPatternId == lesson.DeliveryPatternId)
                  .Include(l => l.Enrollments)
                  .ToListAsync();

                foreach (var l in lessonList)
                {
                    l.Status = Lesson.LessonStatuses.Cancelled;
                }
            }

            // Notify members about the cancellation
            lesson.CreateLessonCancelNotification(_context, title, message, member.Id);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Redirect to the lesson list or details page
        }



        // Change lesson status - one or many
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int lessonId, LessonStatuses newStatus)
        {

            //TODO : Testing by creating method in delivery pattern to change status of lesson/lessons

            var lesson = await _context.Lessons
                              .Include(l => l.DeliveryPattern)
                              .FirstAsync(l => l.Id == lessonId);

            if (lesson == null)
            {
                return NotFound();
            }

            if (!lesson.DeliveryPattern.IsCourse)
            {
                TempData["ChangeLessonStatus"] = $"Status of Lesson id: {lesson.Id} has been updated successfully!";
                lesson.Status = newStatus;
            }
            else if (lesson.DeliveryPattern.IsCourse)
            {
                TempData["ChangeCourseStatus"] = "This lesson belongs to a course. Changing lesson's status effect all corresponding lessons.";
                var lessonlist = _context.Lessons.Where(l => l.DeliveryPatternId == lesson.DeliveryPatternId).ToList();
                foreach (var l in lessonlist)
                {
                    l.Status = newStatus;
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Lessons", new { id = lesson.DeliveryPatternId }); // Redirect to the lesson details page or wherever you need
        }




        //................. Code for Lesson Details ..................................
        [HttpGet]
        [Route("DeliveryPattern/LessonDetails/{id}")]
        public async Task<IActionResult> LessonDetails(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.DeliveryPattern)
                    .ThenInclude(l => l.Professional)
                    .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Member)
                .Include(l => l.Topic) //Include Topic data
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
            {
                return BadRequest("Lesson not found");
            }

            var lessonDetails = new LessonDetails(lesson);

            return View(lessonDetails);

            //return RedirectToAction("Index");

        }

        //-----------------   Code to Enrol member (in single Lesson while course is False or multiple lessons while Course is true) ----------------------------

        public async Task<IActionResult> Enrol(int id)
        {
            var member = await _context.FindMemberAsync(User);
            if (member == null)
            {
                return NotFound("Member not found.");
            }

            var lesson = await _context
                    .Lessons
                    .Include(l => l.DeliveryPattern)
                        .ThenInclude(dp => dp.Professional)
                     .Include(l => l.DeliveryPattern)
                        .ThenInclude(dp => dp.Lessons)
                    .Include(l => l.Enrollments)
                    .Include(l => l.Timetable.Organisation)
                    .FirstAsync(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            try
            {
                var enrolments = lesson.Enrol(member); //enrolments - list of 1(IsCourse:false) to many enrolments(IsCourse: True) for 1 member
                await _context.SaveChangesAsync(); // Save the changes to the database                             
                TempData["SuccessMessage"] = "You have been enrolled successfully.";  // Set success message
                return RedirectToAction("DisplayMyEnrolments"); //?? Redirect to another page after successful enrollment
                //return View();
            }
            catch (ApplicationException ex)
            {
                // Storing the error message in TempData
                TempData["ErrorMessage"] = ex.Message;
                // return RedirectToAction("Index");
                return RedirectToAction("DisplayMyEnrolments");
            }

        }

        // Display All enrolments for the user
        public async Task<IActionResult> DisplayMyEnrolments()
        {
            var member = await _context.FindMemberAsync(User);
            var enrolments = await _context.Enrollments.Where(e => e.MemberId == member.Id).Include(e => e.Lesson.DeliveryPattern).ToListAsync();

            return View(member);
        }

        // Withdraw from Lesson
        [HttpPost]
        public async Task<IActionResult> WithdrawFromLesson(int lessonId)
        {

            var member = await _context.FindMemberAsync(User);    // find member

            if (member == null)
            {
                return NotFound("Member not found");
            }

            var lesson = await _context.Lessons
                .Include(l => l.DeliveryPattern)
                .Include(l => l.Enrollments)
                .FirstOrDefaultAsync(l => l.Id == lessonId); // Find the lesson

            if (lesson == null)
            {
                return BadRequest("Lesson not found");  // TODO: diff between badrequest and notfound WebAPI/ MVC
            }


            if (!lesson.DeliveryPattern.IsCourse)
            {
                var enrolment = await _context.Enrollments.FirstOrDefaultAsync(e => e.MemberId == member.Id && e.LessonId == lesson.Id && e.Status == Data.Enrollment.Statuses.Active);
                if (enrolment == null)
                {
                    throw new ApplicationException("No active enrolment found for this member in this lesson.");
                }
                enrolment.Status = Enrollment.Statuses.Withdrawn; // Set the enrollment status to Withdrawn
                lesson.EnrolledCount--;   // Decrease the enrolled count

                TempData["LessonWithdraw"] = $"You have successfully been withdrawn from the lesson {lesson.Name}";
            }
            else
            {
                var lessonList = await _context.Lessons
                    .Where(l => l.DeliveryPatternId == lesson.DeliveryPatternId)
                    .Include(l => l.Enrollments)
                    .ToListAsync();

                foreach (var l in lessonList)
                {
                    var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.MemberId == member.Id && e.LessonId == l.Id && e.Status == Data.Enrollment.Statuses.Active);
                    if (enrollment != null)
                    {

                        enrollment.Status = Enrollment.Statuses.Withdrawn; // Set the enrollment status to Withdrawn                
                        l.EnrolledCount--; // Decreasing enrolled count for this lesson
                    }
                }
                // Set message for course withdrawal
                var courseLessonIds = string.Join(", ", lessonList.Select(l => l.Id));
                TempData["CourseWithdraw"] = $"You have successfully withdrawn from the entire course. Lesson IDs: {courseLessonIds}";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("DisplayMyEnrolments");   // Redirect back to the member's enrollments page
        }


        //Display enrolments list for a lesson
        public async Task<IActionResult> DisplayLessonEnrolments(int id)
        {

            //            // Fetch enrollments for the specified lessonId
            var enrollmentsList = await _context.Enrollments
                .Include(e => e.Lesson)
                .Include(e => e.Member)
                .Where(e => e.LessonId == id).ToListAsync();

            if (!enrollmentsList.Any())
            {
                ViewData["Message"] = "No enrolments found for this lesson.";
              
            }
         
                return View(enrollmentsList);                  

        }
    }
}
