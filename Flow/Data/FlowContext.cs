using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Flow.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace Flow.Data
{
    public class FlowContext : IdentityDbContext<ApplicationUser>
    {
        public FlowContext(DbContextOptions<FlowContext> options) : base(options)
        {
        }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamRole> TeamRoles { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectRole> ProjectRoles { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define relationships

            builder.Entity<Project>()
                .HasMany(p => p.ProjectRoles)
                .WithOne(pr => pr.Project)
                .HasForeignKey(pr => pr.ProjectId);

            builder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId);
        }
    }
}
