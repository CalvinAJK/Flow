using System.ComponentModel.DataAnnotations;

namespace Flow.Data
{
    public class OrganizationRole
    {
        public int Id { get; set; }

        public int? OrganizationId { get; set; }

        public string? UserId { get; set; }

        public string? Role { get; set; }

        // Navigation property for Organization
        public Organization? Organization { get; set; }
    }
}
