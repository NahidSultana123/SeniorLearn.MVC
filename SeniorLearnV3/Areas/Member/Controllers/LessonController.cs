//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using SeniorLearn.Web.Areas.Member.Controllers;
//using SeniorLearnV3.Areas.Member.Models.Lesson;
//using SeniorLearnV3.Data;
//using SeniorLearnV3.Data.Identity;
//using static SeniorLearnV3.Data.Lesson;
//using System.Net;
//using System.Security.Cryptography;
//using System.Security.Policy;
//using AutoMapper.Execution;
//using Microsoft.AspNetCore.Identity;
//using SeniorLearnV3.Areas.Member.Models.Enrollment;

//namespace SeniorLearnV3.Areas.Member.Controllers
//{

//    public class LessonController : MemberAreaController
//    {
//        private readonly UserManager _userManager;
//        public LessonController(ApplicationDbContext context, UserManager userManager) : base(context)
//        {
//            _userManager = userManager;
//        }

//        //[HttpGet]
//        public async Task<IActionResult> Index()
//        {

//            var userId = _userManager.GetUserId(User);

//            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);

//            if (userRole == null)
//            {
//                return BadRequest("User Not Found");
//            }

//            // TODO: create constructor in DetailsLesson class and remove lengthy codes from controller

//            var lessons = await _context.Lessons
//                .Include(l => l.Professional) // Load the related Professional entity
//                        .ThenInclude(p => p.User) // Load the related User entity
//                        .ThenInclude(u => u.Member) // Load the related Member entity
//                .Include(l => l.Topic)
//                .Where(l => l.ProfessionalId == userRole.Id)
//                .Select(l => new LessonDetails(l)) // Use the constructor to map the lesson
//                .ToListAsync();

//            return View(lessons);
//            //// TODO: create constructor in DetailsLesson class and remove lengthy codes from controller

//            //var lessons = await _context.Lessons.Select(l => new LessonDetails(l, member.FirstName)).ToListAsync();



//        }

//        [HttpGet]
//        public async Task<IActionResult> Create()
//        {
//            var topics = await _context.Topics.ToListAsync();

//            var model = new CreateLesson
//            {
//                Topics = new SelectList(topics, "Id", "Name"),  // Ensure Id is used for value and Name for display
//            };

//            return View(model);
//        }



//        [HttpPost]
//        public async Task<IActionResult> Create(Member.Models.Lesson.CreateLesson model)
//        {
//            var member = await _context.FindMemberAsync(User);

//            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == member.UserId);

//            if (userRole == null)
//            {
//                return BadRequest("You must be a Professional to create a lesson.");
//            }

//            int pId = userRole.Id;

//            if (ModelState.IsValid)
//            {
//                var lesson = new Lesson
//                {
//                    Name = model.Name,
//                    Description = model.Description,
//                    StartDate = model.StartDate,
//                    DurationInMinutes = model.DurationInMinutes,
//                    Status = (Lesson.LessonStatuses)model.LessonStatusId, // Map the selected status
//                    DeliveryMode = (Lesson.DeliveryModes)model.DeliveryModeId, // Map the selected delivery mode
//                    Address = model.Address,
//                    URL = model.URL,
//                    Capacity = model.Capacity,
//                    EnrolledCount = 0,
//                    TopicId = model.TopicId, // Assign selected topic
//                    ProfessionalId = pId// Professional creating the lesson
//                };

//                // Add and save lesson to database
//                _context.Lessons.Add(lesson);
//                _context.SaveChanges();

//                return RedirectToAction("Index"); // Redirect to the lesson list or other appropriate page
//            }

//            // Re-populate dropdown lists in case of validation errors

//            model.Topics = new SelectList(await _context.Topics.ToListAsync(), "Id", "Name");

//            return View(model);
//        }

//        [HttpGet]
//        public async Task<IActionResult> Update(int id)
//        {


//            // var lesson = await _context.Lessons.Include(l => l.Topic) // Include topic information if needed
//            // .FirstOrDefaultAsync(l => l.Id == id);

//            var lesson = await _context.Lessons.Include(l => l.Topic).FirstOrDefaultAsync(l => l.Id == id);
//            if (lesson == null)
//            {
//                return NotFound();
//            }

//            var topics = await _context.Topics.ToListAsync();

//            var model = new UpdateLesson
//            {
//                Id = lesson.Id,
//                Name = lesson.Name,
//                Description = lesson.Description,
//                StartDate = lesson.StartDate,
//                DurationInMinutes = lesson.DurationInMinutes,
//                LessonStatusId = (int)lesson.Status, // Assuming Status is an enum
//                DeliveryModeId = (int)lesson.DeliveryMode, // Assuming DeliveryMode is an enum
//                Address = lesson.Address,
//                URL = lesson.URL,
//                Capacity = lesson.Capacity,
//                TopicId = lesson.TopicId,
//                Topics = new SelectList(topics, "Id", "Name"), // Populate the topics
//                //ProfessionalId = lesson.ProfessionalId // Professional who created the lesson
//            };

//            return View(model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Update(UpdateLesson model)
//        {
//            if (ModelState.IsValid)
//            {
//                var lesson = await _context.Lessons.FindAsync(model.Id);
//                if (lesson == null)
//                {
//                    return NotFound();
//                }

//                //// Update lesson properties
//                lesson.Name = model.Name;
//                lesson.Description = model.Description;
//                lesson.StartDate = model.StartDate;
//                lesson.DurationInMinutes = model.DurationInMinutes;
//                lesson.Status = (Lesson.LessonStatuses)model.LessonStatusId; // Map the selected status
//                lesson.DeliveryMode = (Lesson.DeliveryModes)model.DeliveryModeId; // Map the selected delivery mode
//                lesson.Address = model.Address;
//                lesson.URL = model.URL;
//                lesson.Capacity = model.Capacity;
//                lesson.TopicId = model.TopicId; // Assign selected topic


//                await _context.SaveChangesAsync();

//                return RedirectToAction("Index"); // Redirect to the lesson list or details page
//            }

//            // Repopulate topics if model state is invalid
//            model.Topics = new SelectList(await _context.Topics.ToListAsync(), "Id", "Name");

//            return View(model);
//        }

//        // This method retrieves enrollments for a specific lesson
//        public async Task<IActionResult> LessonEnrollments(int id)
//        {
//            // Fetch enrollments for the specified lessonId
//            var enrollments = await _context.Enrollments
//                .Include(e => e.Lesson)
//                .Include(e => e.Member)
//                .Where(e => e.LessonId == id) // Filter by lessonId
//                .Select(e => new EnrollmentDetails
//                {
//                    EnrollmentId = e.Id,
//                    LessonId = e.LessonId,
//                    LessonName = e.Lesson.Name,
//                    LessonStartDate = e.Lesson.StartDate,
//                    MemberName = e.Member.FirstName + " " + e.Member.LastName,// Assuming Member has User entity
//                    Status = e.Status.ToString(), // Convert enum status to string
//                    EnrolledCount = e.Lesson.EnrolledCount,
//                    AvailableSpace = e.Lesson.Capacity - e.Lesson.EnrolledCount
//                })
//                .ToListAsync();

//            return View(enrollments); // Pass the list of enrollments to the view
//        }

//        [HttpPost]
//        // Reactivating status from Withdrawn to Active and increase enroll count
//        public IActionResult ActivateEnrollment(int enrollmentId)
//        {
//            // Fetch the enrollment by its ID
//            var enrollment = _context.Enrollments.
//                Include(e => e.Lesson).
//                FirstOrDefault(e => e.Id == enrollmentId);

//            if (enrollment == null)
//            {
//                TempData["Error"] = "Enrollment not found.";
//                //return RedirectToAction("LessonEnrollments");
//                return RedirectToAction("Index");
//            }

//            // Check if the enrollment is currently Withdrawn
//            if (enrollment.Status == Enrollment.Statuses.Withdrawn)
//            {
//                // Get the related lesson
//                var lesson = enrollment.Lesson;

//                // Check if there is space available in the lesson
//                if (lesson.EnrolledCount < lesson.Capacity)
//                {
//                    // Update enrollment status to Active
//                    enrollment.Status = Enrollment.Statuses.Active;

//                    // Increment the EnrolledCount for the lesson
//                    lesson.EnrolledCount++;

//                    // Save the changes
//                    _context.SaveChanges();

//                    TempData["Message"] = "Enrollment has been activated successfully.";
//                }
//                else
//                {
//                    TempData["Error"] = "Cannot activate enrollment. The lesson is already at full capacity.";
//                }
//            }
//            else
//            {
//                TempData["Error"] = "Enrollment is not in a Withdrawn state.";
//            }

//            //return RedirectToAction("LessonEnrollments", new { lessonId = enrollment.LessonId });
//            return RedirectToAction("Index");
//        }
//    }
//}
