using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorLearnV3.Data;

namespace SeniorLearnV3.Areas.Administration.Controllers
{
    [Area("Administration"), Authorize(Policy = "ActiveRole", Roles = "ADMINISTRATION")]

    //[Area("Administration"), Authorize(Roles = "ADMINISTRATION")]
    public class AdministrationAreaController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public AdministrationAreaController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
