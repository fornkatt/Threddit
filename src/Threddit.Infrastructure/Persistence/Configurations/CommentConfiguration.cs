using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comment_Comments", "threddit");
        
        builder.HasKey(c => c.Id);
        
        builder.HasIndex(p => p.CommentedAt);

        builder.HasIndex(c => c.Score);

        builder.HasIndex(c => c.ReplyCount);

        builder.Property(c => c.Content)
            .HasMaxLength(Comment.Limits.MaxContentLength);
        
        builder.Property(c => c.ImageUrl)
            .HasMaxLength(Comment.Limits.MaxImageUrlLength);
        
        builder.Property(c => c.DeleteReason)
            .HasMaxLength(Comment.Limits.MaxDeleteReasonLength);
        
        builder.HasOne(c => c.CommentedBy)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.CommentedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public sealed class SavedCommentConfiguration : IEntityTypeConfiguration<SavedComment>
{
    public void Configure(EntityTypeBuilder<SavedComment> builder)
    {
        builder.ToTable("Comment_SavedComments", "threddit");
        
        builder.HasKey(sc => sc.Id);
        
        builder.HasIndex(sc => new { sc.UserId, sc.CommentId }).IsUnique();
        
        builder.HasOne(sc => sc.Comment)
            .WithMany(c => c.Saves)
            .HasForeignKey(sc => sc.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(sc => sc.User)
            .WithMany(u => u.SavedComments)
            .HasForeignKey(sc => sc.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CommentVoteConfiguration : IEntityTypeConfiguration<CommentVote>
{
    public void Configure(EntityTypeBuilder<CommentVote> builder)
    {
        builder.ToTable("Comment_CommentVotes", "threddit");
        
        builder.HasKey(cv => cv.Id);
        
        builder.HasIndex(cv => new { cv.UserId, cv.CommentId }).IsUnique();
        
        builder.HasOne(cv => cv.Comment)
            .WithMany(c => c.Votes)
            .HasForeignKey(cv => cv.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(cv => cv.User)
            .WithMany(u => u.CommentVotes)
            .HasForeignKey(cv => cv.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}