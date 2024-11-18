using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator;
using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;
using SeniorLearnV3.Extensions;
using System;

//TODO: Implement application logic for managing members and roles 
namespace SeniorLearnV3.Areas.Administration.Controllers
{
    public class MemberController : AdministrationAreaController
    {
        private readonly UserManager _userManager;
        private readonly IMapper _mapper;


        public MemberController(ApplicationDbContext context,UserManager userManager,IMapper mapper) 
            : base(context)
        {
            _userManager = userManager;
            _mapper = mapper;   
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? active)
        {
            var query = _context.Members.AsQueryable();

            if (active.HasValue)
            {
                bool a = active.Value;
                query = query.Where(m => _context.UserRoles.OfType<Standard>().Any(ur => ur.UserId == m.UserId && ur.Active == a));
            }
            var members = await query.OrderBy(m => m.LastName)
                    .ThenBy(m => m.FirstName)
                .ToArrayAsync();

            return View(members);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
         
        [HttpPost] 
        public async Task<IActionResult> Register(Models.Member.Register m)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = m.Email, Email = m.Email };
                var password = new Password().Next();
                
                password = "s123!@#";
                var result = await _userManager.CreateAsync(user,password);

                if (result.Succeeded)
                {
                    var organisation = await _context.Organisations.FirstAsync();
                    organisation.RegisterMember(user, m.FirstName, m.LastName, m.Email);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");     
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(m); 
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int id)
        {
            var member = await _context.FindMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<Models.Member.Manage>(member);
            return View(model);      
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Models.Member.Manage member)
        {
            var original = await _context.FindMemberAsync(member.Id);
            if (original == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                original.FirstName = member.FirstName;
                original.LastName = member.LastName;
                original.DateOfBirth = member.DateOfBirth;
                original.RenewalDate = member.RenewalDate;
                original.OutstandingFees = member.OutstandingFees;
                await _context.SaveChangesAsync();

            }
            return RedirectToAction("Manage", new { Id = member.Id }); 
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStandardRole(int id,int active)
        {
            var member = await _context.FindMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }
           
            member.UpdateStandardRole(active.ToBool(), $"ROLE: {(active.ToBool() ? "A" : "Dea")}ctivated");
            await _context.SaveChangesAsync();
            return RedirectToAction("Manage", new { id }); 
       
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfessionalRole(int id, int active, int renewal)
        {
            var member = await _context.FindMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            member.UpdateProfessionalRole("",active.ToBool(), $"ROLE: {(active.ToBool() ? "A" : "Dea")}ctivated");
            
            if(active.ToBool() && renewal > 0)
            {
                var now = DateTime.Now;
                var proposed = now; 
                switch (renewal)
                {
                    case 3:
                        proposed = proposed.GetProfessionalTrialRenewalDate();
                        break;
                    case 12:
                        proposed = proposed.GetNextAnnualRenewalDate();
                        break; 
                    default:
                        break; 
                }
                member.RenewalDate = proposed > member.RenewalDate ? proposed : member.RenewalDate;
            }
          
            await _context.SaveChangesAsync();
            return RedirectToAction("Manage", new { id });

        }


        [HttpPost]
        public async Task<IActionResult> GrantHonoraryRole(int id)
        {
            var member = await _context.FindMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            member.UpdateHonoraryRole(true, $"ROLE: Honorary (granted)"); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Manage", new { id });
        }


    }
}
