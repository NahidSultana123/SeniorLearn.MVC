using Microsoft.AspNetCore.Identity;
using System.Data;

namespace SeniorLearnV3.Data.Identity
{
    public class UserRole : IdentityUserRole<string>
    {
        //TODO: Use nested referenced enum (strongly typed mechanism for role types)
        public enum RoleTypes { STANDARD, PROFESSIONAL, HONORARY, OTHER };
        public int Id { get; set; }
        public User User { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public virtual RoleTypes RoleType => RoleTypes.OTHER;
        public virtual int Order => 0;
        public bool Active { get; set; }
        public List<RoleUpdate> Updates { get; set; } = new();

    }

    public class Standard : UserRole
    {
        public DateTime RegistrationDate { get; set; }
        public override RoleTypes RoleType => RoleTypes.STANDARD;
        public override int Order => 1;
    }

    public class Professional : UserRole
    {
        public string Discipline { get; set; } = "";
        public override RoleTypes RoleType => RoleTypes.PROFESSIONAL;
        public override int Order => 2;

        // TODO: Change in Sprint 3 when Delivery pattern will be added
        public List<DeliveryPattern> DeliveryPatterns { get; set; } = new();

        public DeliveryPattern AddDeliveryPattern(
        string name,
        DateTime startsOn,
        DeliveryPattern.DeliveryPatternTypes patternType,
        bool isCourse = false,
        Lesson.DeliveryModes deliveryMode = Lesson.DeliveryModes.OnPremises,
        DateTime endOn = default,
        IList<DayOfWeek> days = default!)
        {

            DeliveryPattern pattern;

            switch (patternType)
            {
                case DeliveryPattern.DeliveryPatternTypes.NonRepeating:
                    pattern = new NonRepeating(name, startsOn, isCourse, deliveryMode);
                    break;
        // public Daily(string name, DateTime startOn, DateTime endOn, bool isCourse, Lesson.DeliveryModes deliveryMode)
                case DeliveryPattern.DeliveryPatternTypes.Daily:
                    pattern = new Daily(name, startsOn, endOn, isCourse, deliveryMode);
                    break;
                case DeliveryPattern.DeliveryPatternTypes.Weekly:
                    pattern = new Weekly(name, startsOn, endOn, isCourse, deliveryMode, days);
                    break;
                default: throw new NotImplementedException("No Delivery Pattern Type found");
            }
            pattern.IsCourse = isCourse;
            pattern.Name = name;
            pattern.Professional = this;
            DeliveryPatterns.Add(pattern);
            return pattern;
        }

    }

    public class Honorary : UserRole
    {
        public string Notes { get; set; } = "";
        public override RoleTypes RoleType => RoleTypes.HONORARY;
        public override int Order => 3;
    }
}

