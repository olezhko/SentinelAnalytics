using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Data.Configurations;

public sealed class CrashReportConfiguration : IEntityTypeConfiguration<CrashReport>
{
    public void Configure(EntityTypeBuilder<CrashReport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ProjectId)
            .IsRequired();

        builder.Property(x => x.SessionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.ExceptionName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.StackTrace)
            .IsRequired()
            .HasMaxLength(8000);

        builder.Property(x => x.UserId)
            .HasMaxLength(100);

        builder.Property(x => x.PropertiesJson)
            .HasMaxLength(4000);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => new { x.ProjectId, x.Severity });
        builder.HasIndex(x => new { x.ProjectId, x.Timestamp });
        builder.HasIndex(x => new { x.ProjectId, x.IsResolved });
        builder.HasIndex(x => new { x.ProjectId, x.IsIgnored });
    }
}