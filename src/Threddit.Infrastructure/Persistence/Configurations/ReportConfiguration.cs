using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence.Configurations;

public sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Report_Reports", "threddit");
        
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.SubThreadId);
        builder.HasIndex(r => r.PostId);
        builder.HasIndex(r => r.TargetPostId);
        builder.HasIndex(r => r.TargetCommentId);
        builder.HasIndex(r => r.TargetUserId);
        builder.HasIndex(r => r.ReportedAt);
        builder.HasIndex(r => new { r.SubThreadId, r.Status });
        builder.HasIndex(r => new { r.PostId, r.Status });
        builder.HasIndex(r => r.Status)
            .HasFilter("[SubThreadId] IS NULL")
            .HasDatabaseName("IX_Report_AdminQueue");
        
        builder.Property(r => r.Message)
            .HasMaxLength(Report.Limits.MaxMessageLength);
        
        builder.Property(r => r.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Category)
            .HasConversion<string>()
            .HasMaxLength(30);
        
        builder.HasOne(r => r.Reporter)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.SubThread)
            .WithMany(st => st.Reports)
            .HasForeignKey(r => r.SubThreadId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(r => r.TargetPost)
            .WithMany()
            .HasForeignKey(r => r.TargetPostId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(r => r.TargetComment)
            .WithMany()
            .HasForeignKey(r => r.TargetCommentId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(r => r.TargetUser)
            .WithMany()
            .HasForeignKey(r => r.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.TargetSubThread)
            .WithMany()
            .HasForeignKey(r => r.TargetSubThreadId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(r => r.TargetDirectMessage)
            .WithMany()
            .HasForeignKey(r => r.TargetDirectMessageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}