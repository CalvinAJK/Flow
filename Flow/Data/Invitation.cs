﻿namespace Flow.Data
{
    public class Invitation
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int InviterId { get; set; }
        public int InvitedId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime DateCreated { get; set; } = DateTime.UtcNow; // Set to current UTC time by default
    }
}
