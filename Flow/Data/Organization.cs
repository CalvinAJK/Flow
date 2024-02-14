using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flow.Data
{
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? IsDeleted { get; set; } = false;

        // Navigation property for OrganizationRoles
        public ICollection<OrganizationRole>? OrganizationRoles { get; set; }
    }
}

