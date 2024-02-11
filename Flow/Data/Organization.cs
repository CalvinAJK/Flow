﻿using System.ComponentModel.DataAnnotations;

namespace Flow.Data
{
    public class Organization
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? IsDeleted { get; set; } = false;

        // Navigation property for OrganizationRoles
        public ICollection<OrganizationRole>? OrganizationRoles { get; set; }
    }
}

