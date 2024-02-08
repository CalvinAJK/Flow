namespace Flow.Data
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation property for TeamRoles
        public ICollection<TeamRole> TeamRoles { get; set; }
    }
}
