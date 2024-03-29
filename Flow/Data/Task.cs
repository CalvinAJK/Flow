﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flow.Data
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public Priority Priority { get; set; }

        public int? ProjectId { get; set; }

        // Navigation property for comments
        public ICollection<Comment>? Comments { get; set; }

        // Navigation property for images
        public ICollection<Image>? Images { get; set; }

        // Navigation property for Project
        public Project? Project { get; set; }
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }
}
