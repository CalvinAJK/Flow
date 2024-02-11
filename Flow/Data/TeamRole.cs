namespace Flow.Data
{
    public class TeamRole
    {
        public int Id { get; set; }

        public int? TeamId { get; set; }

        public string? UserId { get; set; }

        public string? Role { get; set; }

        // Navigation property for Team
        public Team? Team { get; set; }
    }
}
