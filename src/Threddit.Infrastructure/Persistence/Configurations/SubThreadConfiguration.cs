using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence.Configurations;

public sealed class SubThreadConfiguration : IEntityTypeConfiguration<SubThread>
{
    public void Configure(EntityTypeBuilder<SubThread> builder)
    {
        builder.ToTable("SubThread_SubThreads", "threddit");
        
        builder.HasKey(st => st.Id);

        builder.HasIndex(st => new { st.Name }).IsUnique();

        builder.HasIndex(st => st.SubscriberCount);

        builder.Property(st => st.Name)
            .HasMaxLength(SubThread.Limits.MaxNameLength);
        
        builder.Property(st => st.Description)
            .HasMaxLength(SubThread.Limits.MaxDescriptionLength);
        
        builder.Property(st => st.BannerImageUrl)
            .HasMaxLength(SubThread.Limits.MaxBannerImageUrlLength);
        
        builder.HasOne(st => st.SubThreadOwner)
            .WithOne(o => o.SubThread)
            .HasForeignKey<SubThreadOwner>(o => o.SubThreadId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(st => st.CreatedBy)
            .WithMany(u => u.CreatedSubThreads)
            .HasForeignKey(st => st.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SubThreadSubscriptionConfiguration : IEntityTypeConfiguration<SubThreadSubscription>
{
    public void Configure(EntityTypeBuilder<SubThreadSubscription> builder)
    {
        builder.ToTable("SubThread_SubThreadSubscriptions", "threddit");
        
        builder.HasKey(st => st.Id);
        
        builder.HasIndex(st => new { st.UserId, st.SubThreadId }).IsUnique();
        
        builder.HasOne(st => st.User)
            .WithMany(u => u.SubThreadSubscriptions)
            .HasForeignKey(st => st.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(st => st.SubThread)
            .WithMany(st => st.Subscribers)
            .HasForeignKey(st => st.SubThreadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SubThreadRuleConfiguration : IEntityTypeConfiguration<SubThreadRule>
{
    public void Configure(EntityTypeBuilder<SubThreadRule> builder)
    {
        builder.ToTable("SubThread_SubThreadRules", "threddit");
        
        builder.HasKey(st => st.Id);
        
        builder.Property(st => st.RuleTitle)
            .HasMaxLength(SubThreadRule.Limits.MaxTitleLength);
        
        builder.Property(st => st.RuleContent)
            .HasMaxLength(SubThreadRule.Limits.MaxContentLength);
        
        builder.HasOne(st => st.SubThread)
            .WithMany(st => st.SubThreadRules)
            .HasForeignKey(st => st.SubThreadId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(st => st.CreatedBy)
            .WithMany()
            .HasForeignKey(st => st.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(st => st.LastUpdatedBy)
            .WithMany()
            .HasForeignKey(st => st.LastUpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}