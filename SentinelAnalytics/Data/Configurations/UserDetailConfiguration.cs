using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Data.Configurations;

public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
{
    public void Configure(EntityTypeBuilder<UserDetail> builder)
    {
        // Primary Key
        builder.HasKey(x => new { x.UserId, x.PlanId});

        // Unique constraint (ensures 1 subscription per user)
        builder.HasIndex(x => x.UserId)
               .IsUnique();
    }
}