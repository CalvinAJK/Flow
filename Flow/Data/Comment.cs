using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flow.Data
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key property referencing the Task
        public int TaskId { get; set; }

        // Navigation property for Task
        public Task? Task { get; set; }

        // Foreign key property referencing the User
        public string UserId { get; set; }
    }
}
