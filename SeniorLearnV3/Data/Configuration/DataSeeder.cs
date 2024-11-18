using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data.Identity;
using SeniorLearnV3.Extensions;
using System.Net;
using System.Security.Policy;
using static SeniorLearnV3.Areas.Member.Models.DeliveryPattern.LessonDetails;
using static SeniorLearnV3.Data.Lesson;
using static SeniorLearnV3.Data.Repeating;


namespace SeniorLearnV3.Data.Configuration
{
    public class DataSeeder
    {
        static int enrolmentId = 10;
        public DataSeeder(ModelBuilder mb)
        {
            mb.Entity<Organisation>().HasData(new Organisation
            {
                Id = 1,
                Name = "SeniorLearn",
                TimetableId = 1,
                StandardMembershipFee = 50.0m, // Initial value for standard members
                ProfessionalMembershipFee = 0.0m, // Professional members are exempt
                HonoraryMembershipFee = 0.0m // Honorary members are exempt
            });
            mb.Entity<Timetable>().HasData(new Timetable { Id = 1, OrganisationId = 1 });

            Identity(mb);
            Topics(mb);
            DeliveryPatternsAndLessons(mb);

        }

        private static void Identity(ModelBuilder mb)
        {

            string adminPassword = "AQAAAAIAAYagAAAAEApS7xqe3eAo0ckko/q1+iU/L3pP/l+3D1wiyT0fYQgS4Ay7VdQoPaYVG276ExEKeQ==";
            string adminSalt = "4G54HDQS7FANMIXF6GWVXO5KJLQFKTLZ";
            string memberPassword = "AQAAAAIAAYagAAAAEMN1xqajYz3XZWBnm/pd4Csh0kgnqoZT8BxNqjpvT4GlSJAuY9xXaNTYthPQ8Ux6mg==";
            string memberSalt = "4G54HDQS7FANMIXF6GWVXO5KJLQFKTLZ";

            mb.Entity<Role>().HasData(
                new Role { Id = "ADMINISTRATION", Name = "ADMINISTRATION", NormalizedName = "ADMINISTRATION", ConcurrencyStamp = "f384153f-aa5b-4560-9daf-e15646ca047a" },
                new Role { Id = "HONORARY", Name = "HONORARY", NormalizedName = "HONORARY", ConcurrencyStamp = "61bcc0ce-a897-49a4-bc04-295a25c78bd3" },
                new Role { Id = "PROFESSIONAL", Name = "PROFESSIONAL", NormalizedName = "PROFESSIONAL", ConcurrencyStamp = "4c91e4f9-2e31-4ed3-9f36-9b5d4ce1ceac" },
                new Role { Id = "STANDARD", Name = "STANDARD", NormalizedName = "STANDARD", ConcurrencyStamp = "fb519d60-4b07-469b-a11c-b7638a33b636" }
            );

            mb.Entity<User>().HasData(
                new User
                {
                    Id = "173ef34b-19c4-48e8-aada-4c3d17bfe57f",
                    UserName = "admin1@seniorlearn.org.au",
                    NormalizedUserName = "ADMIN1@SENIORLEARN.ORG.AU",
                    Email = "admin1@seniorlearn.org.au",
                    NormalizedEmail = "ADMIN1@SENIORLEARN.ORG.AU",
                    EmailConfirmed = true,
                    PasswordHash = adminPassword,
                    SecurityStamp = adminSalt,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new User
                {
                    Id = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e4",
                    UserName = "senior@citizen.email",
                    NormalizedUserName = "SENIOR@CITIZEN.EMAIL",
                    Email = "senior@citizen.email",
                    NormalizedEmail = "SENIOR@CITIZEN.EMAIL",
                    EmailConfirmed = true,
                    PasswordHash = memberPassword,
                    SecurityStamp = memberSalt,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new User
                {
                    Id = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e5",
                    UserName = "professional@citizen.email",
                    NormalizedUserName = "PROFESSIONAL@CITIZEN.EMAIL",
                    Email = "professional@citizen.email",
                    NormalizedEmail = "PROFESSIONAL@CITIZEN.EMAIL",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEMN1xqajYz3XZWBnm/pd4Csh0kgnqoZT8BxNqjpvT4GlSJAuY9xXaNTYthPQ8Ux6mg==",
                    SecurityStamp = "ARG6AAJ3NIQWXIEC357FJM7WVEODGVUT",
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                }
            );

            var now = DateTime.Now;
            var renewal = now.GetNextMonthlyRenewalDate();
            string notes = "INITIAL ROLE ACTIVATION";

            mb.Entity<UserRole>().HasData(new UserRole { Id = 1, Active = true, UserId = "173ef34b-19c4-48e8-aada-4c3d17bfe57f", RoleId = "ADMINISTRATION" });
            mb.Entity<Standard>().HasData(new Standard { Id = 2, Active = true, UserId = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e4", RoleId = "STANDARD", RegistrationDate = now });
            mb.Entity<Standard>().HasData(new Standard { Id = 3, Active = true, UserId = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e5", RoleId = "STANDARD", RegistrationDate = now });
            mb.Entity<Professional>().HasData(new Professional { Id = 4, Active = true, UserId = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e5", RoleId = "PROFESSIONAL", Discipline = "All" });

            int roleUpdateId = 1;

            mb.Entity<RoleUpdate>().HasData(
                new { Id = roleUpdateId++, UserRoleId = 1, Active = true, Timestamp = now, RenewaDate = DateTime.MaxValue, Notes = notes },
                new { Id = roleUpdateId++, UserRoleId = 2, Active = true, Timestamp = now, RenewaDate = renewal, Notes = notes },
                new { Id = roleUpdateId++, UserRoleId = 3, Active = true, Timestamp = now, RenewaDate = renewal, Notes = notes },
                new { Id = roleUpdateId++, UserRoleId = 4, Active = true, Timestamp = now, RenewaDate = renewal.AddMonths(2), Notes = notes }
           );

            mb.Entity<Member>().HasData(
                new Member
                {
                    Id = 1,
                    DateOfBirth = DateTime.Now.AddYears(-63),
                    Email = "senior@citizen.email",
                    FirstName = "Senior",
                    LastName = "Citizen",
                    OrganisationId = 1,
                    OutstandingFees = 0.0m,
                    RenewalDate = renewal,
                    UserId = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e4"
                },
                new Member
                {
                    Id = 2,
                    DateOfBirth = DateTime.Now.AddYears(-72),
                    Email = "professional@citizen.email",
                    FirstName = "Professional",
                    LastName = "Citizen",
                    OrganisationId = 1,
                    OutstandingFees = 0.0m,
                    RenewalDate = renewal.AddMonths(2),
                    UserId = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e5"
                }
            );

            int memberIdBase = 10;
            int userRoleIdBase = 5;
            var random = new Random();

            for (int i = 0; i < 200; i++)
            {
                var userId = Guid.NewGuid().ToString();
                var userName = $"senior-{i}@citizen.com";
                var normalizedUserName = userName.ToUpper();
                var renewalDate = renewal.AddMonths(i % 12);

                mb.Entity<User>().HasData(new User
                {
                    Id = userId,
                    UserName = userName,
                    NormalizedUserName = normalizedUserName,
                    Email = userName,
                    NormalizedEmail = normalizedUserName,
                    EmailConfirmed = true,
                    PasswordHash = memberPassword,
                    SecurityStamp = memberSalt,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                });

                mb.Entity<Member>().HasData(new Member
                {
                    Id = memberIdBase + i,
                    DateOfBirth = now.AddYears(-30 - (i % 30)),
                    Email = userName,
                    FirstName = $"Senior-{i}",
                    LastName = $"Citizen-{i}",
                    OrganisationId = 1,
                    OutstandingFees = i % 3 == 0 ? 0 : random.Next(0, 51),
                    RenewalDate = renewalDate,
                    UserId = userId
                });

                bool active = i >= 10;
                mb.Entity<Standard>().HasData(new Standard
                {
                    Id = userRoleIdBase++,
                    Active = active,
                    UserId = userId,
                    RoleId = "STANDARD"
                });

                mb.Entity<RoleUpdate>().HasData(new { Id = roleUpdateId++, UserRoleId = userRoleIdBase - 1, Active = active, Timestamp = now, RenewaDate = renewalDate, Notes = notes });

                if (i < 180)
                {
                    if (i < 100)
                    {

                        mb.Entity<Professional>().HasData(new Professional
                        {
                            Id = userRoleIdBase++,
                            Active = i < 80,
                            UserId = userId,
                            RoleId = "PROFESSIONAL",
                            Discipline = "Discipline" + (i % 5).ToString()
                        });
                        mb.Entity<RoleUpdate>().HasData(new { Id = roleUpdateId++, UserRoleId = userRoleIdBase - 1, Active = active, Timestamp = now, RenewaDate = renewalDate, Notes = notes });
                    }
                    else if (i < 120)
                    {
                        mb.Entity<Honorary>().HasData(new Honorary
                        {
                            Id = userRoleIdBase++,
                            Active = true,
                            UserId = userId,
                            RoleId = "HONORARY"
                        });
                        mb.Entity<RoleUpdate>().HasData(new { Id = roleUpdateId++, UserRoleId = userRoleIdBase - 1, Active = true, Timestamp = now, RenewaDate = DateTime.MaxValue, Notes = notes });
                    }
                }
            }

        }

        private void Topics(ModelBuilder mb)
        {
            mb.Entity<Topic>().HasData(
                    new Topic { Id = 1, OrganisationId = 1, Name = "Mathematics", Description = "An essential subject focused on developing skills in arithmetic, algebra, geometry, and basic calculus to build analytical and problem-solving abilities." },
                    new Topic { Id = 2, OrganisationId = 1, Name = "English Language", Description = "Develops reading, writing, and communication skills, with an emphasis on grammar, vocabulary, and literary comprehension." },
                    new Topic { Id = 3, OrganisationId = 1, Name = "Science", Description = "A broad field covering foundational concepts in biology, chemistry, and physics, aimed at fostering scientific literacy and inquiry skills." },
                    new Topic { Id = 4, OrganisationId = 1, Name = "History", Description = "Explores key events and developments in human history, focusing on social, political, and economic changes across civilizations." },
                    new Topic { Id = 5, OrganisationId = 1, Name = "Geography", Description = "Studies the Earth’s landscapes, environments, and how humans interact with them, including physical geography and human geography." },
                    new Topic { Id = 6, OrganisationId = 1, Name = "Civics", Description = "Focuses on the role of citizens in government, legal systems, and civic responsibility, fostering an understanding of democracy and public participation." },
                    new Topic { Id = 7, OrganisationId = 1, Name = "Information Technology", Description = "Introduces fundamental concepts of computing, including software, hardware, and internet technologies, promoting digital literacy." },
                    new Topic { Id = 8, OrganisationId = 1, Name = "Physical Education", Description = "Promotes physical fitness, health, and wellness through exercises and activities designed to improve motor skills, strength, and endurance." },
                    new Topic { Id = 9, OrganisationId = 1, Name = "Art", Description = "Encourages creativity and self-expression through the study of various visual art forms, techniques, and art history." },
                    new Topic { Id = 10, OrganisationId = 1, Name = "Music", Description = "Focuses on developing musical skills through vocal and instrumental training, as well as understanding music theory and appreciation." },
                    new Topic { Id = 11, OrganisationId = 1, Name = "Economics", Description = "Examines how individuals, businesses, and governments allocate resources, focusing on microeconomics, macroeconomics, and economic systems." },
                    new Topic { Id = 12, OrganisationId = 1, Name = "Literature", Description = "Involves the study of written works, analyzing themes, characters, and narrative techniques in classical and contemporary literature." },
                    new Topic { Id = 13, OrganisationId = 1, Name = "Social Studies", Description = "Explores society, culture, and human interactions, covering topics like sociology, anthropology, and political science." },
                    new Topic { Id = 14, OrganisationId = 1, Name = "Environmental Science", Description = "Studies the impact of humans on the environment, focusing on sustainability, conservation, and ecosystems." },
                    new Topic { Id = 15, OrganisationId = 1, Name = "Business Studies", Description = "Introduces students to business concepts such as entrepreneurship, marketing, management, and finance." },
                    new Topic { Id = 16, OrganisationId = 1, Name = "Psychology", Description = "Examines human behavior and mental processes, including topics such as cognition, emotion, and development." },
                    new Topic { Id = 17, OrganisationId = 1, Name = "Philosophy", Description = "Encourages critical thinking and the study of fundamental questions related to existence, knowledge, ethics, and logic." },
                    new Topic { Id = 18, OrganisationId = 1, Name = "Foreign Languages", Description = "Develops communication skills in languages other than the native language, promoting cross-cultural understanding and proficiency." },
                    new Topic { Id = 19, OrganisationId = 1, Name = "Chemistry", Description = "Investigates the properties and behavior of matter, focusing on atomic structure, chemical reactions, and materials science." },
                    new Topic { Id = 20, OrganisationId = 1, Name = "Physics", Description = "Explores the laws governing energy, matter, and the physical forces of the universe, with applications in technology and natural phenomena." },
                    new Topic { Id = 21, OrganisationId = 1, Name = "Biology", Description = "Studies living organisms, focusing on their structure, function, evolution, and interactions with their environment." },
                    new Topic { Id = 22, OrganisationId = 1, Name = "Ethics", Description = "Explores moral philosophy and decision-making, emphasizing the concepts of right, wrong, justice, and individual responsibility." },
                    new Topic { Id = 23, OrganisationId = 1, Name = "Geology", Description = "Examines the Earth's physical structure and substances, studying processes like rock formation, earthquakes, and plate tectonics." },
                    new Topic { Id = 24, OrganisationId = 1, Name = "Statistics", Description = "Introduces data analysis and probability, providing tools to interpret data and make informed decisions based on statistical models." },
                    new Topic { Id = 25, OrganisationId = 1, Name = "World Religions", Description = "World Religions explores various global belief systems, their teachings, practices, and impacts on societies." }
                  );
        }


        private void DeliveryPatternsAndLessons(ModelBuilder mb)
        {
            var topics = new List<Topic>
            {
                new Topic(1, "Mathematics"),
                new Topic(2, "English Language"),
                new Topic(3, "Science"),
                new Topic(4, "History"),
                new Topic(5, "Geography"),
                new Topic(6, "Civics"),
                new Topic(7, "Information Technology"),
                new Topic(8, "Physical Education"),
                new Topic(9, "Art"),
                new Topic(10, "Music"),
                new Topic(11, "Economics"),
                new Topic(12, "Literature"),
                new Topic(13, "Social Studies"),
                new Topic(14, "Environmental Science"),
                new Topic(15, "Business Studies"),
                new Topic(16, "Psychology"),
                new Topic(17, "Philosophy"),
                new Topic(18, "Foreign Languages"),
                new Topic(19, "Chemistry"),
                new Topic(20, "Physics"),
                new Topic(21, "Biology"),
                new Topic(22, "Ethics"),
                new Topic(23, "Geology"),
                new Topic(24, "Statistics"),
                new Topic(25, "World Religions")
            };

            Random random = new();
            var startDate = DateTime.Now;
            int lessonIdBase = 1;


            var daily = new List<object>();
            var weekly = new List<object>();

            for (int i = 1; i <= 50; i++) 
            {
                var topic = topics[random.Next(topics.Count)];
                var start = startDate.AddMonths(i % 12);
                bool isCourse = random.Next() % 7 == 0;
                var mode = random.Next(2) == 1 ? Lesson.DeliveryModes.Online : Lesson.DeliveryModes.OnPremises;
                string name = $"DELIVERY PATTERN, TOPIC: {topic.Name}, MODE: {mode}, IS COURSE: {isCourse}";
                DateTime? end = null; // Initialize end as nullable


                DeliveryPattern pattern;
         

                if (random.Next(0, 2) == 1)  //if Daily
                {
                    end = start.AddDays(7 * random.Next(1, 6));
                    //Daily(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
                    pattern = new Daily($"{name}, Repeat: Daily", start, end!.Value, isCourse, mode);
                
                }
                else
                {
                    //Weekly(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode, IList<DayOfWeek> days = default!)
                    end = start.AddDays(7 * random.Next(1, 6));
                    var daysOfWeek = new List<DayOfWeek>
                            {
                                DayOfWeek.Wednesday,
                                DayOfWeek.Friday
                            }; // Example days
                    pattern = new Weekly($"{name}, Repeat: Weekly", start, end!.Value, isCourse, mode, daysOfWeek);

                }
   

                // Seeding the `pattern` entity

                pattern.Id = i;
                //// Ensures the number is even as the script inserts this range for active professional members;
                pattern.ProfessionalId = random.Next(8, 40) & ~1;

                //-------------------------------Switch--------------------------------------------------------------
                switch (pattern.PatternType)
                {
                    case DeliveryPattern.DeliveryPatternTypes.Daily:
                        var dp = (Daily)pattern;
                        var x = new
                        {
                            Id = i,
                            pattern.ProfessionalId,
                            pattern.IsCourse,
                            pattern.Name,
                            pattern.DeliveryMode,
                            pattern.StartOn,
                            EndOn = dp.EndOn > dp.StartOn.AddDays(7) ? dp.EndOn : dp.StartOn.AddDays(7)
                        };

                        mb.Entity<Daily>().HasData(x);
                        daily.Add(x);
                        break;

                    case DeliveryPattern.DeliveryPatternTypes.Weekly:
                        var wp = (Weekly)pattern;
                        var y = new
                        {
                            Id = i,
                            pattern.ProfessionalId,
                            pattern.IsCourse,
                            pattern.Name,
                            pattern.DeliveryMode,
                            pattern.StartOn,
                            EndOn = wp.EndOn > wp.StartOn.AddDays(7) ? wp.EndOn : wp.StartOn.AddDays(7),
                            wp.Sunday,
                            wp.Monday,
                            wp.Tuesday,
                            wp.Wednesday,
                            wp.Thursday,
                            wp.Friday,
                            wp.Saturday
                        };
                        mb.Entity<Weekly>().HasData(y);
                        weekly.Add(y);
                        break;
                }

                name = $"LESSON: {pattern.Name}";

                pattern.Professional = new Professional { Active = true, Id = pattern.ProfessionalId };
                var timetable = new Timetable { Id = 1, OrganisationId = 1 };
                //GenerateLessons(Timetable timetable, string name, string description, int durationInMinutes, int capacity, Topic topic, string address, string url)
                pattern.GenerateLessons(timetable, name, name, 10 * random.Next(3, 13), 10, topic, "", "");

                foreach (var lesson in pattern.Lessons)
                {
                    lesson.DeliveryPatternId = pattern.Id;
                    lesson.TimeTableId = timetable.Id;

                    lesson.Status = LessonStatuses.Scheduled;
                    lesson.DeliveryMode = pattern.DeliveryMode;

                    if (lesson.DeliveryMode == Lesson.DeliveryModes.OnPremises)
                    {
                        lesson.Address = "Lesson OnPrem Location (somewhere)";
                        lesson.URL = "";
                    }
                    else if (lesson.DeliveryMode == Lesson.DeliveryModes.Online)
                    {
                        lesson.URL = $"http://seniorlearn.azurewebsites.net/meeting/{lessonIdBase}";
                        lesson.Address = "";

                    }
                    lesson.EnrolledCount = 5;

                    var l = new
                    {
                        Id = lessonIdBase,
                        lesson.Name,
                        lesson.Description,
                        lesson.Start,
                        lesson.DurationInMinutes,
                        lesson.TimeTableId,
                        lesson.DeliveryPatternId,
                        lesson.Status,
                        lesson.DeliveryMode,
                        lesson.Address,
                        lesson.URL,
                        lesson.Capacity,
                        lesson.EnrolledCount,
                        TopicId = topic.Id,
                    };

                    mb.Entity<Lesson>().HasData(l);
                    lessonIdBase++;
                    lesson.Id = l.Id;
                }


                // Seed 5 enrollments for the current lesson
                for (int j = 0; j < 5; j++)
                {
                    //var randomMemberId = random.Next(10, 51); prior

                    int randomMemberId;

                    do
                    {
                        randomMemberId = random.Next(10, 51); // Generating a random MemberId between 10 and 50
                    }
                    while (randomMemberId == pattern.ProfessionalId);

           

                    if (pattern.IsCourse)
                    {
                        // Enroll the same member in all lessons if it's a course
                        foreach (var courseLesson in pattern.Lessons)
                        {
                            var courseEnrollment = new
                            {
                                Id = enrolmentId++, // Unique Enrollment ID
                                LessonId = courseLesson.Id,
                                MemberId = randomMemberId,
                                EnrollmentDate = DateTime.Now,
                                Status = Enrollment.Statuses.Active // Default status: Active
                            };

                            mb.Entity<Enrollment>().HasData(courseEnrollment);
                        }
                        // break; // Avoiding duplicating member enrollments across multiple lessons in the loop
                    }
                    else
                    {
                        // Enroll only in the current lesson
                        var enrollment = new
                        {
                            Id = enrolmentId++, // Unique Enrollment ID
                            LessonId = pattern.Lessons[random.Next(0, pattern.Lessons.Count)].Id,//l.Id,
                            MemberId = randomMemberId,
                            EnrollmentDate = DateTime.Now,
                            Status = Enrollment.Statuses.Active // Default status: Active
                        };

                        mb.Entity<Enrollment>().HasData(enrollment);
                    }


                }

          

            }



        }


    }
}

