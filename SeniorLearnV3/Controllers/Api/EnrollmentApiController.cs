using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SeniorLearnV3.Controllers.Api
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollemntApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager _userManager;
        public EnrollemntApiController(ApplicationDbContext context, UserManager userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        // Fetches the count of active enrollments for both Standard and Professional members.
        // It first filters active members based on their role (Standard/Professional),
        // then counts the number of active enrollments for each role. The result is returned
        // as a JSON object with separate counts for Standard and Professional active enrollments.

        [HttpGet("active-enrollments")]
        public async Task<IActionResult> GetActiveEnrollments()
        {
            int standardAECount = 0;
            int professionalAECount = 0;

            // TODO
            var activeStandardMembers = await _context.UserRoles
                    .OfType<Standard>() // Filter to Standard roles
                    .Where(ur => ur.Active) // Check if the role is active
                    .Select(ur => ur.UserId) // Get UserIds of active Standard members
                    .ToListAsync();

            var standardActiveEnrollments = await _context.Enrollments
                   .Include(e => e.Member) // Ensure Members are loaded
                   .Where(e => e.Status == Enrollment.Statuses.Active &&
                               activeStandardMembers.Contains(e.Member.UserId)) // Filter by UserId
                   .ToListAsync();

            if (standardActiveEnrollments != null)
            {
                standardAECount = standardActiveEnrollments.Count();
            }


        
            // Step 1: First, get all active Professional members
            var activeProfessionalMembers = await _context.UserRoles
                .OfType<Professional>() // Filter to Professional roles
                .Where(ur => ur.Active) // Check if the role is active
                .Select(ur => ur.UserId) // Get UserIds of active Professional members
                .ToListAsync();

            // Step 2: Now, filter enrollments based on the active professional members
            var professionalActiveEnrollments = await _context.Enrollments
                .Include(e => e.Member) // Ensure Members are loaded
                .Where(e => e.Status == Enrollment.Statuses.Active &&
                            activeProfessionalMembers.Contains(e.Member.UserId)) // Filter by UserId
                .ToListAsync();

            if (professionalActiveEnrollments != null)
            {
                professionalAECount = professionalActiveEnrollments.Count();
            }


            var data = new
            {
                standard = standardAECount,
                professional = professionalAECount
            };

            return Ok(data);
        }

    }
}
