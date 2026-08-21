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

        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }


        public DbSet<PricingPlan> PricingPlans { get; set; }

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


            modelBuilder.Entity<PricingPlan>().HasData(
               new PricingPlan
               {
                   Id = Guid.Parse("f0000000-0000-0000-0000-000000000001"),
                   Name = "Free",
                   Price = 0,
                   MaxProjects = 1,
                   MaxEventsPerMonth = 1000,
                   MaxCrashesPerMonth = 100,
                   MaxTeamMembersPerProject = 2,
                   Description = "Perfect for side projects and hobbyists."
               },
               new PricingPlan
               {
                   Id = Guid.Parse("f0000000-0000-0000-0000-000000000002"),
                   Name = "Pro",
                   Price = 19,
                   MaxProjects = 10,
                   MaxEventsPerMonth = 100000,
                   MaxCrashesPerMonth = 5000,
                   MaxTeamMembersPerProject = 10,
                   Description = "For growing teams and production apps."
               },
               new PricingPlan
               {
                   Id = Guid.Parse("f0000000-0000-0000-0000-000000000003"),
                   Name = "Max",
                   Price = 100,
                   MaxProjects = int.MaxValue,
                   MaxEventsPerMonth = int.MaxValue,
                   MaxCrashesPerMonth = int.MaxValue,
                   MaxTeamMembersPerProject = int.MaxValue,
                   Description = "Enterprise-grade scale and support."
               }
           );

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(new AuditSaveChangesInterceptor());
    }
}
