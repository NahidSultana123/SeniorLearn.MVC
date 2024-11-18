using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data.Configuration;
using SeniorLearnV3.Data.Identity;
using System.Security.Claims;


namespace SeniorLearnV3.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public override DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleUpdate> RoleUpdates { get; set; }
        public DbSet<DeliveryPattern> DeliveryPatterns { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Topic> Topics { get; set; }
   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new EntityMapper(modelBuilder);
            new DataSeeder(modelBuilder);

        }

        public async Task<Member?> FindMemberAsync(int id)
        {
            return await Members
                   .Include(m => m.User)
                       .ThenInclude(u => u.Roles)
                           .ThenInclude(r => r.Updates)
                    .Include(m => m.Organisation)
                        .ThenInclude(o => o.Timetable)
                   .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Member> FindMemberAsync(ClaimsPrincipal principal)
        {
            return await Members
                   .Include(m => m.User)
                       .ThenInclude(u => u.Roles)
                           .ThenInclude(r => r.Updates)
                    .Include(m => m.Organisation)
                        .ThenInclude(o => o.Timetable)
                   .FirstAsync(m => m.User.UserName == principal.Identity!.Name);
        }



    }


}
