using System.ComponentModel.DataAnnotations;

namespace Flow.Data
{
    public class OrganizationRole
    {
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }

        // Navigation property for Organization
        public Organization Organization { get; set; }
    }
}
