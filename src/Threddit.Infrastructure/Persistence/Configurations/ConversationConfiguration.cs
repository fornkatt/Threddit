using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence.Configurations;

public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversation_Conversations", "threddit");
        
        builder.HasKey(c => c.Id);
        
        builder.HasIndex(c => new { c.InitiatorId, c.RecipientId }).IsUnique();
        
        builder.HasOne(c => c.Initiator)
            .WithMany(c => c.InitiatedConversations)
            .HasForeignKey(c => c.InitiatorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(c => c.Recipient)
            .WithMany(u => u.ReceivedConversations)
            .HasForeignKey(c => c.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class GroupConversationConfiguration : IEntityTypeConfiguration<GroupConversation>
{
    public void Configure(EntityTypeBuilder<GroupConversation> builder)
    {
        builder.ToTable("Conversation_GroupConversations", "threddit");

        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(GroupConversation.Limits.MaxNameLength);
        
        builder.HasOne(g => g.CreatedBy)
            .WithMany()
            .HasForeignKey(g => g.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class GroupConversationMemberConfiguration : IEntityTypeConfiguration<GroupConversationMember>
{
    public void Configure(EntityTypeBuilder<GroupConversationMember> builder)
    {
        builder.ToTable("Conversation_GroupConversationMembers", "threddit");

        builder.HasKey(m => new { m.UserId, m.GroupConversationId, m.JoinedAt });
        
        builder.HasOne(m => m.User)
            .WithMany(u => u.GroupConversationMemberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(m => m.GroupConversation)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class DirectMessageConfiguration : IEntityTypeConfiguration<DirectMessage>
{
    public void Configure(EntityTypeBuilder<DirectMessage> builder)
    {
        builder.ToTable("Conversation_DirectMessages", "threddit");
        
        builder.HasKey(dm => dm.Id);

        builder.Property(dm => dm.Content)
            .HasMaxLength(DirectMessage.Limits.MaxContentLength);
        
        builder.HasOne(dm => dm.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(dm => dm.ConversationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(dm => dm.GroupConversation)
            .WithMany(gc => gc.Messages)
            .HasForeignKey(dm => dm.GroupConversationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(dm => dm.Sender)
            .WithMany()
            .HasForeignKey(dm => dm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(dm => dm.ParentMessage)
            .WithMany()
            .HasForeignKey(dm => dm.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class DirectMessageReadConfiguration : IEntityTypeConfiguration<DirectMessageRead>
{
    public void Configure(EntityTypeBuilder<DirectMessageRead> builder)
    {
        builder.ToTable("Conversation_DirectMessageReads", "threddit");

        builder.HasKey(dmr => new { dmr.DirectMessageId, dmr.UserId });
        
        builder.HasOne(dmr => dmr.DirectMessage)
            .WithMany(dm => dm.Reads)
            .HasForeignKey(dmr => dmr.DirectMessageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(dmr => dmr.User)
            .WithMany()
            .HasForeignKey(dmr => dmr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}