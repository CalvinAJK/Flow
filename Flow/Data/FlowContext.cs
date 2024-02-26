using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Flow.Models;
using Microsoft.AspNetCore.Identity;

namespace Flow.Data
{
    public class FlowContext : IdentityDbContext<IdentityUser>
    {
        public FlowContext(DbContextOptions<FlowContext> options) : base(options)
        {
        }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamRole> TeamRoles { get; set; }


    }
}
