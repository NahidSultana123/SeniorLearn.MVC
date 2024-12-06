using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Areas.Member.Controllers;
using SeniorLearnV3.Areas.Member.Models.Home;
using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;
using System.Security.Claims;

namespace SeniorLearnV3.Areas.Member.Controllers
{
    public class HomeController : MemberAreaController
    {
        public HomeController(ApplicationDbContext context) : base(context)
        {

        }

        // GET: HomeController
        // Retrieves unread notifications for the logged-in member and displays them.
        public async Task<ActionResult> Index()
        {
            var member = await _context.FindMemberAsync(User);
            var notifications = await _context.Notifications
                .Where(n => n.ToMemberId == member.Id && n.IsRead==false)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            ViewData["NotificationCount"] = notifications.Count;
            return View(member);
        }

        // Retrieves and displays the profile of the logged-in member.
        [HttpGet]
        public async Task<ActionResult> ViewProfile()
        {
            var member = await _context.FindMemberAsync(User);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // Retrieves the member's current profile details and prepares them for editing.
        [HttpGet]
        public async Task<ActionResult> EditProfile()
        {
            var member = await _context.FindMemberAsync(User);
            if (member == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName
            };

            return View(model);

        }


        // Handles the submission of the profile edit form, updating the member's details in the database if valid.
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == model.Id);

                if (member != null)
                {
                    // Update the member's first name and last name
                    member.FirstName = model.FirstName;
                    member.LastName = model.LastName;

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Redirect to a confirmation page or member's profile page
                    return RedirectToAction("Index", "Home");
                }

                // If the member is not found, you could handle it here
                return NotFound();
            }

            // If model state is invalid, return the same view with error messages
            return View(model);
        }

        // Retrieves and displays the payment history for the currently logged-in member.
        [HttpGet]
        public async Task<ActionResult> PaymentHistory()
        {
            var member = await _context.FindMemberAsync(User);
            if (member == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.Where(p => p.MemberId == member.Id)
                .OrderByDescending(p => p.PaymentDate).ToListAsync();

            return View(payment);

        }



        // Fetches and displays a list of notifications for the currently logged-in member.
        public async Task<IActionResult> Notifications()
        {
            var member = await _context.FindMemberAsync(User);
            var notifications = await _context.Notifications
                .Where(n => n.ToMemberId == member.Id)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(notifications);
        }


        // Marks a notification as read or unread and saves the change to the database.
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = !notification.IsRead;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Notifications");
        }

 
    }
}
