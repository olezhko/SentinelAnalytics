using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Data.Interceptor;

namespace SentinelAnalytics.Data
{
    public class SentinelDbContext : IdentityDbContext
    {
        public SentinelDbContext(DbContextOptions<SentinelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<MobileEvent> MobileEvents { get; set; }
        public DbSet<CrashReport> CrashReports { get; set; }


        public DbSet<UserSubscription> UserSubscriptions { get; set; }

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(new AuditSaveChangesInterceptor());
    }
}
