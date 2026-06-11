using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User_Users", "threddit");
        
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.TotalScore);
        
        builder.Property(u => u.UserName)
            .HasMaxLength(User.Limits.MaxUsernameLength);
        
        builder.Property(u => u.Email)
            .HasMaxLength(User.Limits.MaxEmailLength);

        builder.Property(u => u.ProfilePicture)
            .HasMaxLength(User.Limits.MaxProfilePictureUrlLength);
        
        builder.Property(u => u.Description)
            .HasMaxLength(User.Limits.MaxDescriptionLength);
    }
}

public sealed class ModeratorConfiguration : IEntityTypeConfiguration<SubThreadModerator>
{
    public void Configure(EntityTypeBuilder<SubThreadModerator> builder)
    {
        builder.ToTable("User_SubThreadModerators", "threddit");
        
        builder.HasKey(m => m.Id);
        
        builder.HasIndex(m => new { m.UserId, m.SubThreadId }).IsUnique();
        
        builder.HasOne(m => m.User)
            .WithMany(u => u.SubThreadModeratorRoles)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(m => m.SubThread)
            .WithMany(s => s.Moderators)
            .HasForeignKey(m => m.SubThreadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class OwnerConfiguration : IEntityTypeConfiguration<SubThreadOwner>
{
    public void Configure(EntityTypeBuilder<SubThreadOwner> builder)
    {
        builder.ToTable("User_SubThreadOwners", "threddit");
        
        builder.HasKey(o => o.Id);
        
        builder.HasOne(o => o.User)
            .WithMany(u => u.SubThreadOwnerRoles)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(o => o.SubThread)
            .WithOne(st => st.SubThreadOwner)
            .HasForeignKey<SubThreadOwner>(o => o.SubThreadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AdminConfiguration : IEntityTypeConfiguration<SiteAdmin>
{
    public void Configure(EntityTypeBuilder<SiteAdmin> builder)
    {
        builder.ToTable("User_SiteAdmins", "threddit");
        
        builder.HasKey(a => a.Id);
        
        builder.HasIndex(a => new { a.UserId }).IsUnique();
        
        builder.HasOne(a => a.User)
            .WithOne(u => u.SiteAdmin)
            .HasForeignKey<SiteAdmin>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SiteOwnerConfiguration : IEntityTypeConfiguration<SiteOwner>
{
    public void Configure(EntityTypeBuilder<SiteOwner> builder)
    {
        builder.ToTable("User_SiteOwner", "threddit");

        builder.HasKey(so => so.SingletonKey);
        
        builder.Property(so => so.SingletonKey)
            .ValueGeneratedNever()
            .HasDefaultValue(1);

        builder.ToTable(t => t
            .HasCheckConstraint("CK_SiteOwner_SingletonKey", "[SingletonKey] = 1"));
        
        builder.HasOne(so => so.User)
            .WithOne(u => u.SiteOwner)
            .HasForeignKey<SiteOwner>(so => so.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BannedSubThreadUserConfiguration : IEntityTypeConfiguration<BannedSubThreadUser>
{
    public void Configure(EntityTypeBuilder<BannedSubThreadUser> builder)
    {
        builder.ToTable("User_BannedSubThreadUsers", "threddit");
        
        builder.HasKey(bu => bu.Id);
        
        builder.HasIndex(bu => new { bu.UserId, bu.SubThreadId }).IsUnique();
        
        builder.Property(bu => bu.Reason)
            .HasMaxLength(BannedSubThreadUser.Limits.MaxReasonLength);
        
        builder.HasOne(bu => bu.User)
            .WithMany(u => u.ReceivedSubThreadBans)
            .HasForeignKey(bu => bu.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(bu => bu.SubThread)
            .WithMany(s => s.BannedUsers)
            .HasForeignKey(bu => bu.SubThreadId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(bu => bu.BannedBy)
            .WithMany(u => u.IssuedSubThreadBans)
            .HasForeignKey(bu => bu.BannedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(bu => bu.LastEditedBy)
            .WithMany()
            .HasForeignKey(bu => bu.LastEditedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BannedSiteUserConfiguration : IEntityTypeConfiguration<BannedSiteUser>
{
    public void Configure(EntityTypeBuilder<BannedSiteUser> builder)
    {
        builder.ToTable("User_BannedSiteUsers", "threddit");

        builder.HasKey(bs => bs.Id);
        
        builder.HasIndex(bs => bs.UserId).IsUnique();
        
        builder.Property(bs => bs.Reason)
            .HasMaxLength(BannedSiteUser.Limits.MaxReasonLength);
        
        builder.HasOne(bs => bs.User)
            .WithOne(u => u.BannedSiteUser)
            .HasForeignKey<BannedSiteUser>(bs => bs.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(bs => bs.BannedBy)
            .WithMany(u => u.IssuedSiteBans)
            .HasForeignKey(bs => bs.BannedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BlockedUserConfiguration : IEntityTypeConfiguration<BlockedUser>
{
    public void Configure(EntityTypeBuilder<BlockedUser> builder)
    {
        builder.ToTable("User_BlockedUsers", "threddit");
        
        builder.HasKey(bu => bu.Id);
        
        builder.HasIndex(bu => new { bu.BlockerId, bu.BlockedId }).IsUnique();
        
        builder.HasOne(bu => bu.Blocker)
            .WithMany(b => b.BlockedUsers)
            .HasForeignKey(bu => bu.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(bu => bu.Blocked)
            .WithMany(b => b.BlockedByUsers)
            .HasForeignKey(bu => bu.BlockedId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class FollowedUserConfiguration : IEntityTypeConfiguration<FollowedUser>
{
    public void Configure(EntityTypeBuilder<FollowedUser> builder)
    {
        builder.ToTable("User_FollowedUsers", "threddit");
        
        builder.HasKey(fu => fu.Id);
        
        builder.HasIndex(fu => new { fu.FollowerId, fu.FollowedId }).IsUnique();
        
        builder.HasOne(fu => fu.Follower)
            .WithMany(fu => fu.FollowedUsers)
            .HasForeignKey(fu => fu.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(fu => fu.Followed)
            .WithMany(fu => fu.FollowedByUsers)
            .HasForeignKey(fu => fu.FollowedId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}