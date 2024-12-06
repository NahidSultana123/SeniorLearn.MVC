using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;
using SeniorLearnV3.Models;
using System.Diagnostics;

namespace SeniorLearn.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager _userManager;

        public HomeController(UserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Redirects the user to different areas based on their role.
        public async Task<IActionResult> RedirectUser()
        {

            if (User.IsInRole("ADMINISTRATION"))
            {
                return RedirectToAction("Index", "Home", new { area = "Administration" });
            }

            if (await _userManager.UserHasActiveRoleAsync(User, UserRole.RoleTypes.STANDARD.ToString()))
            {
                return RedirectToAction("Index", "Home", new { area = "Member" });
            }
            if (await _userManager.UserHasActiveRoleAsync(User, UserRole.RoleTypes.OTHER.ToString()))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return BadRequest();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
