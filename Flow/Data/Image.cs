using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flow.Data
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Url { get; set; }

        // Foreign key property referencing the Task
        public int TaskId { get; set; }

        // Navigation property for Task
        public Task Task { get; set; }
    }
}
