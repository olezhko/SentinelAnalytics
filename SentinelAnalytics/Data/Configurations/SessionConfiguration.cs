using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Data.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever(); // Guid generated in CLR

        builder.Property(x => x.ProjectId)
            .IsRequired();

        // Properties
        builder.Property(x => x.DeviceId)
               .IsRequired()
               .HasMaxLength(128);

        builder.Property(x => x.Country)
               .IsRequired()
               .HasMaxLength(8); // ISO country code (e.g. LT, USA)

        builder.Property(x => x.Language)
               .IsRequired()
               .HasMaxLength(10); // e.g. en, en-US

        builder.Property(x => x.AppVersion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OsVersion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DeviceModel)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes (important for analytics queries)
        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.Country);
        builder.HasIndex(x => x.Language);

        // Relationships
        builder.HasMany(x => x.Crashes)
               .WithOne(x => x.Session)
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Events)
               .WithOne(x => x.Session)
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.NoAction);


        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}