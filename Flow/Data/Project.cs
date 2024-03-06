using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flow.Data
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int TeamId { get; set; }

        // Navigation property for Team
        public Team Team { get; set; }

        // Navigation property for ProjectRole
        public ICollection<ProjectRole> ProjectRoles { get; set; }
    }
}
