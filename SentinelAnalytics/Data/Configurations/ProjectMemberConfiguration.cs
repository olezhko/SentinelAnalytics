using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Data.Configurations;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.HasIndex(pm => new { pm.ProjectId, pm.UserEmail }).IsUnique();

        builder.Property(pm => pm.UserEmail)
            .IsRequired()
            .HasMaxLength(255);
    }
}