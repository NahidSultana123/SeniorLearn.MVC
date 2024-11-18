using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data;
using System.Security.Claims;

namespace SeniorLearnV3.Areas.Member.Controllers
{
    [Area("Member"), Authorize(Policy = "ActiveRole", Roles = "STANDARD")]
    public class MemberAreaController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public MemberAreaController(ApplicationDbContext context)
        {
            _context = context;
        }



    }
}
