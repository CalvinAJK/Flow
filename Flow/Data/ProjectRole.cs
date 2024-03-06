using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flow.Data
{
    public class ProjectRole
    {
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        public string? UserId { get; set; }

        public string? Role { get; set; }

        // Navigation property for Project
        public Project? Project { get; set; }
    }
}
