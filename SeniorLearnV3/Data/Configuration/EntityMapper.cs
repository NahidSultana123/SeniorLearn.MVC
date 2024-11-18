using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data.Identity;
using System.Reflection.Emit;
//using System.Reflection.Emit;
//using SeniorLearnV3.Data.Views;

namespace SeniorLearnV3.Data.Configuration
{
    public class EntityMapper
    {
        public EntityMapper(ModelBuilder mb)
        {
            mb.HasDefaultSchema("org");

            mb.Entity<Organisation>(o =>
            {
                o.HasOne(o => o.Timetable)
                    .WithOne(t => t.Organisation)
                    .HasForeignKey<Timetable>(t => t.OrganisationId)
                    .OnDelete(DeleteBehavior.Restrict);

                o.HasMany(o => o.Topics)
                    .WithOne(t => t.Organisation)
                    .HasForeignKey(t => t.OrganisationId)
                    .OnDelete(DeleteBehavior.Restrict);

                o.Property(o => o.StandardMembershipFee)
                    .HasPrecision(5, 2);

                o.Property(o => o.ProfessionalMembershipFee)
                    .HasPrecision(5, 2);

                o.Property(o => o.HonoraryMembershipFee)
                    .HasPrecision(5, 2);
            });


            mb.Entity<User>(u =>
            {
                u.HasOne(u => u.Member)
                .WithOne(m => m.User)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.Restrict);

                u.HasMany(u => u.Roles)
                    .WithOne()
                    .HasPrincipalKey(u => u.Id)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });



            mb.Entity<UserRole>(ur =>
            {
                ur.HasKey(ur => ur.Id);
                ur.UseTptMappingStrategy();

                ur.HasOne(ur => ur.User)
                .WithMany(u => u.Roles);

                ur.HasIndex(ur => new { ur.UserId, ur.RoleId })
                    .IsUnique();

                ur.HasOne(ur => ur.Role)
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId);

                mb.Entity<Standard>();

                mb.Entity<Professional>();

                mb.Entity<Professional>(p =>
                {

                    p.HasMany(p => p.DeliveryPatterns)
                      .WithOne(dp => dp.Professional)
                      .HasForeignKey(l => l.ProfessionalId)
                      .OnDelete(DeleteBehavior.Restrict);
                });

                mb.Entity<Honorary>();

            });

            mb.Entity<Member>(m =>
            {
                m.Property(p => p.OutstandingFees).HasColumnType("decimal(5,2)");

                m.Property(m => m.RowVersion).IsRowVersion();

                m.Ignore(m => m.Roles);

                m.HasMany(m => m.Enrollments)    // Added in Sprint 2
                    .WithOne(e => e.Member)
                    .OnDelete(DeleteBehavior.Restrict);

                m.HasMany(m => m.Payments)
                  .WithOne(p => p.Member)
                  .OnDelete(DeleteBehavior.Restrict);
            

            });
           
            
            mb.Entity<Payment>(p =>
            {
                p.ToTable("Payments", schema: "finance");
                // The relationship with Lesson will be configured later.

                // Configuring the precision and scale for Amount in Payment
                //p.Property(p => p.Amount)
                //.HasPrecision(5, 2); // Precision: 18 digits, 2 decimal places
                p.Property(p => p.Amount).HasColumnType("decimal(5,2)"); // Same as .HasPrecision(5, 2);?
            });

       

            mb.Entity<Topic>(t =>
            {
                t.ToTable("Topics", schema: "timetable");
                t.HasMany(t => t.Lessons)
                    .WithOne(l => l.Topic)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TODO: later stage -done
            mb.Entity<Timetable>(t =>
            {
                t.ToTable("Timetables", schema: "timetable");
                t.HasMany(t => t.Lessons)
                    .WithOne(l => l.Timetable)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //TODO: DeliveryPattern mapping......Done
            mb.Entity<DeliveryPattern>(dp => {

                dp.ToTable("DeliveryPatterns", schema: "timetable");

                dp.UseTphMappingStrategy();

                dp.HasOne(dp => dp.Professional)
                    .WithMany(p => p.DeliveryPatterns)
                    .HasForeignKey(dp => dp.ProfessionalId)
                    .OnDelete(DeleteBehavior.Restrict);

                dp.Property(dp => dp.DeliveryMode)
                    .HasColumnName("DeliveryMode")
                    .HasConversion<int>();

                mb.Entity<Daily>();
                mb.Entity<NonRepeating>();
                mb.Entity<Weekly>();
            });

            //Lesson mapping Sprint2
            mb.Entity<Lesson>(l =>
            {
                l.ToTable("Lessons", schema: "timetable");
                l.HasKey(l => l.Id);

                l.HasOne(l => l.Topic)
                    .WithMany(t => t.Lessons)
                    .HasForeignKey(l => l.TopicId)
                    .OnDelete(DeleteBehavior.Restrict);
              
                l.HasOne(l => l.DeliveryPattern)
                    .WithMany(dp => dp.Lessons)
                    .HasForeignKey(l => l.DeliveryPatternId)
                    .OnDelete(DeleteBehavior.Restrict);

                //l.HasOne(l => l.Professional)
                //    .WithMany(p => p.lessons)  // Assuming a Professional can create multiple lessons
                //    .HasForeignKey(l => l.ProfessionalId)
                //    .OnDelete(DeleteBehavior.Restrict);

                l.HasMany(l => l.Enrollments)
                    .WithOne(e => e.Lesson)
                    .HasForeignKey(e => e.LessonId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Other configurations
                l.Property(l => l.Status)
                    .HasConversion<int>();

                l.Property(l => l.DeliveryMode)
                    .HasConversion<int>()
                    .IsRequired();

                l.Property(l => l.Capacity)
                    .IsRequired();

                l.Property(l => l.EnrolledCount)
                    .IsRequired();
                // Indexes (for faster searching)
                l.HasIndex(l => l.Name);
                l.HasIndex(l => l.Start);
            });
        

            // Enrolment mapping
            mb.Entity<Enrollment>(e =>
            {
                e.ToTable("Enrollments", schema: "timetable");
                e.HasKey(e => e.Id);

                e.HasOne(e => e.Member)
                    .WithMany(m => m.Enrollments)
                    .HasForeignKey(e => e.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.Lesson)
                    .WithMany(l => l.Enrollments)
                    .HasForeignKey(e => e.LessonId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Property configuration for EnrollmentDate
                e.Property(e => e.EnrollmentDate)
                    .IsRequired(); // Set the EnrollmentDate as required

                // Enum to int conversion for Status
                e.Property(e => e.Status)
                    .HasConversion<int>()
                    .IsRequired(); // Convert enum Statuses to int and make it required
            });

            mb.Entity<Notification>(n =>
            {
                n.Property(e => e.NotificationType)
                 .HasConversion<int>(); // Maps enum to an integer in the database

                n.HasOne(e => e.FromMember)
                 .WithMany(m => m.SentNotifications)
                 .HasForeignKey(e => e.FromMemberId)
                 .OnDelete(DeleteBehavior.Restrict);

                n.HasOne(e => e.ToMember)
                 .WithMany(m => m.ReceivedNotifications)
                 .HasForeignKey(e => e.ToMemberId)
                 .OnDelete(DeleteBehavior.Restrict);
            });


        }
    }
}
