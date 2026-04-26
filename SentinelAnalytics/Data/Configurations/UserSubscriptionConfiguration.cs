using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Data.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever(); // because Guid.NewGuid() is assigned in CLR

        // UserId
        builder.Property(x => x.UserId)
               .IsRequired()
               .HasMaxLength(450); // default IdentityUser PK length

        // Flags (optional but explicit)
        builder.Property(x => x.NotifyOnCritical)
               .IsRequired();

        builder.Property(x => x.NotifyOnError)
               .IsRequired();

        builder.Property(x => x.NotifyOnRegression)
               .IsRequired();

        // Relationship (1:1)
        builder.HasOne(x => x.User)
               .WithOne()
               .HasForeignKey<UserSubscription>(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint (ensures 1 subscription per user)
        builder.HasIndex(x => x.UserId)
               .IsUnique();
    }
}