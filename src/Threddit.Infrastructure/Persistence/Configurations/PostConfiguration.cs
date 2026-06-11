using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence.Configurations;

public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Post_Posts", "threddit");
        
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Score);
        
        builder.HasIndex(p => p.CommentCount);

        builder.HasIndex(p => new { p.SubThreadId, p.Title })
            .IsUnique();
        
        builder.HasIndex(p => new { p.SubThreadId, p.Slug })
            .IsUnique();

        builder.HasIndex(p => p.PostedAt);
        
        builder.HasQueryFilter(p => !p.IsDeleted);
        
        builder.Property(p => p.Title)
            .HasMaxLength(Post.Limits.MaxTitleLength);
        
        builder.Property(p => p.Slug)
            .HasMaxLength(Post.Limits.MaxSlugLength);

        builder.Property(p => p.Content)
            .HasMaxLength(Post.Limits.MaxContentLength);
        
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(Post.Limits.MaxImageUrlLength);
        
        builder.Property(p => p.DeleteReason)
            .HasMaxLength(Post.Limits.MaxDeleteReasonLength);
        
        builder.HasOne(p => p.PostedBy)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.PostedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.SubThread)
            .WithMany(t => t.Posts)
            .HasForeignKey(p => p.SubThreadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class PinnedSubThreadPostConfiguration : IEntityTypeConfiguration<PinnedSubThreadPost>
{
    public void Configure(EntityTypeBuilder<PinnedSubThreadPost> builder)
    {
        builder.ToTable("Post_PinnedSubThreadPosts", "threddit");
        
        builder.HasKey(ps => ps.Id);
        
        builder.HasOne(ps => ps.Post)
            .WithMany()
            .HasForeignKey(ps => ps.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(ps => ps.PinnedBy)
            .WithMany()
            .HasForeignKey(ps => ps.PinnedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ps => ps.SubThread)
            .WithMany(st => st.PinnedPosts)
            .HasForeignKey(ps => ps.SubThreadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SavedPostConfiguration : IEntityTypeConfiguration<SavedPost>
{
    public void Configure(EntityTypeBuilder<SavedPost> builder)
    {
        builder.ToTable("Post_SavedPosts", "threddit");
        
        builder.HasKey(sp => sp.Id);
        
        builder.HasIndex(sp => new { sp.UserId, sp.PostId }).IsUnique();
        
        builder.HasOne(sp => sp.Post)
            .WithMany(p => p.Saves)
            .HasForeignKey(sp => sp.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(sp => sp.User)
            .WithMany(u => u.SavedPosts)
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PostVoteConfiguration : IEntityTypeConfiguration<PostVote>
{
    public void Configure(EntityTypeBuilder<PostVote> builder)
    {
        builder.ToTable("Post_PostVotes", "threddit");
        
        builder.HasKey(pv => pv.Id);
        
        builder.HasIndex(pv => new { pv.PostId, pv.UserId }).IsUnique();
        
        builder.HasOne(pv => pv.Post)
            .WithMany(p => p.Votes)
            .HasForeignKey(pv => pv.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(pv => pv.User)
            .WithMany(u => u.PostVotes)
            .HasForeignKey(pv => pv.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PostViewConfiguration : IEntityTypeConfiguration<PostView>
{
    public void Configure(EntityTypeBuilder<PostView> builder)
    {
        builder.ToTable("Post_PostViews", "threddit");
        
        builder.HasKey(pv => pv.Id);
        
        builder.HasIndex(pv => new { pv.PostId, pv.UserId }).IsUnique();
        
        builder.HasOne(pv => pv.Post)
            .WithMany()
            .HasForeignKey(pv => pv.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(pv => pv.User)
            .WithMany(u => u.ViewedPostHistory)
            .HasForeignKey(pv => pv.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}