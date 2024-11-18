namespace SeniorLearnV3.Data.Identity
{
    public class RoleUpdate
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public DateTime? RenewaDate { get; set; }

        // Foreign key for UserRole
        public int UserRoleId { get; set; }  // Foreign key property
        public UserRole UserRole { get; set; } = default!;  // Navigation property
    }
}
