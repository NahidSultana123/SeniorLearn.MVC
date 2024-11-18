using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;

namespace SeniorLearnV3.Areas.Administration.Controllers
{
    public class HomeController : AdministrationAreaController
    {
        private readonly UserManager _userManager;
        private readonly RoleManager<Role> _roleManager;
        public HomeController(ApplicationDbContext context, UserManager userManager, RoleManager<Role> roleManager) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {        
            return View();
        }
    }
}
