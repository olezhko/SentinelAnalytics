using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Data.Configurations;

public sealed class MobileEventConfiguration : IEntityTypeConfiguration<MobileEvent>
{
    public void Configure(EntityTypeBuilder<MobileEvent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ProjectId)
            .IsRequired();

        builder.Property(x => x.SessionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EventName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.PropertiesJson)
            .HasMaxLength(4000);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => new { x.ProjectId, x.EventName });
    }
}