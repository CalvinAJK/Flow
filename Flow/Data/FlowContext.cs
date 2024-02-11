using Microsoft.EntityFrameworkCore;

namespace Flow.Data
{
    public class FlowContext : DbContext
    {
        public FlowContext(DbContextOptions<FlowContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamRole> TeamRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity relationships, constraints, etc.
            modelBuilder.Entity<OrganizationRole>()
                .HasKey(or => new { or.Id, or.OrganizationId, or.UserId });

            modelBuilder.Entity<TeamRole>()
                .HasKey(tr => new { tr.Id, tr.TeamId, tr.UserId });
        }
    }
}
