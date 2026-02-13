using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Data
{
    public class SentinelDbContext : IdentityDbContext
    {
        public SentinelDbContext(DbContextOptions<SentinelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<MobileEvent> MobileEvents { get; set; }
        public DbSet<CrashReport> CrashReports { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ApiKey)
                .IsUnique();

            modelBuilder.Entity<MobileEvent>()
                .HasIndex(e => e.SessionId);

            modelBuilder.Entity<CrashReport>()
                .HasIndex(c => c.SessionId);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SentinelDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
