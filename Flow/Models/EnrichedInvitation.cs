namespace Flow.Models
{
    public class EnrichedInvitation
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string InviterEmail { get; set; }
        public string InvitedEmail { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }

}
