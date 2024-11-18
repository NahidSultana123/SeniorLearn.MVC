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



        public async Task<IActionResult> Notifications()
        {
            var member = await _context.FindMemberAsync(User);
            var notifications = await _context.Notifications
                .Where(n => n.ToMemberId == member.Id)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(notifications);
        }

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

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
