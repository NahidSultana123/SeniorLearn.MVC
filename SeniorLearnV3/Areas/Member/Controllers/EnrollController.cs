//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SeniorLearn.Web.Areas.Member.Controllers;
//using SeniorLearnV3.Areas.Member.Models.Lesson;
//using SeniorLearnV3.Data;
//using SeniorLearnV3.Data.Identity;

//namespace SeniorLearnV3.Areas.Member.Controllers
//{
//    public class EnrollController : MemberAreaController
//    {
//        private readonly UserManager _userManager;
//        public EnrollController(ApplicationDbContext context, UserManager userManager) : base(context)
//        {
//            _userManager = userManager;
//        }

//        //[HttpGet]
//        public async Task<IActionResult> Index()
//        {
//             // var a = await _context.FindMemberAsync(User);


//            var lessons = await _context.Lessons           
//                .Include(l => l.Professional) // Load the related Professional entity
//                    .ThenInclude(p => p.User) // Load the related User entity
//                    .ThenInclude(u => u.Member) // Load the related Member entity
//                .Include(l => l.Topic)
//           .Select(l => new LessonDetails(l)) // Use the constructor to map the lesson
//           .ToListAsync();

//            return View(lessons);

//        }

//       // Let user enrol in a lesson
//        public async Task<IActionResult> Enrol(int id)
//        {
//            var member = await _context.FindMemberAsync(User);
//            //var member = await _context.Members.FindAsync(User);

//            if (member == null)
//            {
//                return NotFound("Member not found.");
//            }
//            //var lesson = await _context.Lessons.FirstOrDefaultAsync(l=>l.Id== lessonId);

//            var lesson = await _context.Lessons.Include(l => l.Enrollments).FirstOrDefaultAsync(l => l.Id == id);

//            if (lesson == null)
//            {
//                return NotFound("Lesson not found.");
//            }

//            try
//            {
//              var enrollments = lesson.EnrollMember(member);
//                await _context.SaveChangesAsync(); // Save the changes to the database
//                                                   // Set success message
//                TempData["SuccessMessage"] = "You have been enrolled successfully.";
//                return RedirectToAction("Index"); // Redirect to another page after successful enrollment
//            }
//            catch (ApplicationException ex)
//            {
//                // Add the exception message to ModelState
//                // Store the error message in TempData
//                TempData["ErrorMessage"] = ex.Message;
//                return RedirectToAction("Index");
//            }

//        }

//        // MyEnrollments - Display User's all Enrollments info

//        public async Task<IActionResult> MyEnrollments()
//        {
//            // Get the current logged-in user
//            var user = await _userManager.GetUserAsync(User);

//            if (user == null)
//            {
//                return NotFound("User not found");
//            }

//            // Fetch the member object with enrollments
//            var member = await _context.Members
//                .Include(m => m.Enrollments)
//                .ThenInclude(e => e.Lesson)
//                .FirstOrDefaultAsync(m => m.UserId == user.Id);

//            if (member == null)
//            {
//                return NotFound("Member not found");
//            }

//            // Pass the enrollments to the view
//            return View(member.Enrollments);
//        }

//        [HttpPost]
//        // Withdraw from a lesson  - WIthdraw message will be displayed on MyEnrollments.cshtm form
//        public async Task<IActionResult> WithdrawFromLesson(int lessonId)
//        {
//            // Get the current logged-in user
//            var user = await _userManager.GetUserAsync(User);

//            if (user == null)
//            {
//                return NotFound("User not found");
//            }

//            // Find the member object
//            var member = await _context.Members
//                .Include(m => m.Enrollments)
//                .ThenInclude(e => e.Lesson)
//                .FirstOrDefaultAsync(m => m.UserId == user.Id);

//            if (member == null)
//            {
//                return NotFound("Member not found");
//            }

//            // Find the lesson
//            var lesson = await _context.Lessons
//                .Include(l => l.Enrollments)
//                .FirstOrDefaultAsync(l => l.Id == lessonId);

//            if (lesson == null)
//            {
//                return NotFound("Lesson not found");
//            }

//            try
//            {
//                // Attempt to withdraw the member
//                lesson.WithdrawMember(member);
//                await _context.SaveChangesAsync(); // Save changes to the database

//                TempData["Message"] = "You have successfully withdrawn from the lesson.";
//            }
//            catch (ApplicationException ex)
//            {
//                TempData["Error"] = ex.Message;
//            }

//            // Redirect back to the member's enrollments page
//            return RedirectToAction("MyEnrollments");  //TODO: check if it is triggering the proper view 
//        }

     



//    }
//}
