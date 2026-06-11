using Threddit.Domain.Common;

namespace Threddit.Domain.Entities;

public sealed class Conversation
{
    private Conversation()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? Initiator { get; init; }
    public Guid? InitiatorId { get; init; }
    public User? Recipient { get; init; }
    public Guid? RecipientId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public bool InitiatorHidden { get; private set; }
    public bool RecipientHidden { get; private set; }
    public bool IsAbandoned => InitiatorHidden && RecipientHidden;

    public IReadOnlyCollection<DirectMessage> Messages => _messages.AsReadOnly();
    
    private readonly List<DirectMessage> _messages = [];

    public void HideForInitiator() => InitiatorHidden = true;
    public void HideForRecipient() => RecipientHidden = true;
    
    public static Conversation Create(User user1, User user2)
    {
        var (initiator, recipient) = user1.Id.CompareTo(user2.Id) < 0
            ? (user1, user2)
            : (user2, user1);
        
        return new Conversation()
        {
            Initiator = initiator,
            InitiatorId = initiator.Id,
            Recipient = recipient,
            RecipientId = recipient.Id,
            InitiatorHidden = false,
            RecipientHidden = false,
        };
    }

    public void Unhide()
    {
        InitiatorHidden = false;
        RecipientHidden = false;
    }
}

public sealed class GroupConversation
{
    public static class Limits
    {
        public const int MaxNameLength = 100;
    }
    
    private GroupConversation()
    {
    }
    
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Name { get; private set; } = null!;
    public User? CreatedBy { get; init; }
    public Guid? CreatedById { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public bool IsAbandoned => _members.Count (m => !m.HasLeft) <= 1;
    
    private readonly List<GroupConversationMember> _members = [];
    public IReadOnlyCollection<GroupConversationMember> Members => _members.AsReadOnly();
    
    private readonly List<DirectMessage> _messages = [];
    public IReadOnlyCollection<DirectMessage> Messages => _messages.AsReadOnly();

    /// <summary>Creates a new group chat instance.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.CreatorNameEmpty"/></item>
    ///     <item><see cref="ErrorType.NameEmpty"/></item>
    ///     <item><see cref="ErrorType.NameTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<GroupConversation> Create(User creator, string name, IEnumerable<User> initialMembers)
    {
        if (string.IsNullOrWhiteSpace(creator.UserName))
            return Result<GroupConversation>.Error("Creator username must be supplied.", ErrorType.CreatorNameEmpty);
        
        if (string.IsNullOrWhiteSpace(name))
            return Result<GroupConversation>.Error("Conversation must have a name.", ErrorType.NameEmpty);
        
        if (name.Length > Limits.MaxNameLength)
            return Result<GroupConversation>
                .Error($"Conversation name cannot exceed {Limits.MaxNameLength} characters.", ErrorType.NameTooLong);
        
        var group = new GroupConversation()
        {
            Name = name,
            CreatedBy = creator,
            CreatedById = creator.Id,
        };
        
        group._members.Add(new GroupConversationMember
        {
            User = creator,
            UserId = creator.Id,
            GroupConversationId = group.Id
        });

        foreach (var member in initialMembers.Where(im => im.Id != creator.Id))
        {
            group._members.Add(new GroupConversationMember
            {
                User = member,
                UserId = member.Id,
                GroupConversationId = group.Id
            });
        }
        
        return Result<GroupConversation>.Success(group);
    }

    /// <summary>Adds a member to a group chat.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyMember"/></item>
    /// </list>
    /// </remarks>
    public Result AddMember(User user)
    {
        if (_members.Any(m => m.UserId == user.Id && !m.HasLeft))
            return Result.Error("User is already a member of this group.", ErrorType.AlreadyMember);
        
        _members.Add(new GroupConversationMember
        {
            User = user,
            UserId = user.Id,
            GroupConversationId = Id
        });
        
        return Result.Success();
    }

    /// <summary>Removes a member from a group chat.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.NotSubscribed"/></item>
    /// </list>
    /// </remarks>
    public Result RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId && !m.HasLeft);
        if (member is null)
            return Result.Error("User is not a member of this group.", ErrorType.NotSubscribed);

        member.Leave();
        return Result.Success();
    }

    /// <summary>Renames a group chat.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.NameEmpty"/></item>
    /// </list>
    /// </remarks>
    public Result Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return Result.Error("Conversation must have a name.", ErrorType.NameEmpty);
        
        Name = newName;
        return Result.Success();
    }
}

public sealed class GroupConversationMember
{
    internal GroupConversationMember()
    {
    }

    public User? User { get; init; }
    public Guid? UserId { get; init; }
    public GroupConversation GroupConversation { get; init; } = null!;
    public Guid GroupConversationId { get; init; }
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; private set; }

    public bool HasLeft => LeftAt.HasValue;
    
    internal void Leave() => LeftAt = DateTime.UtcNow;
}

public sealed class DirectMessage
{
    public static class Limits
    {
        public const int MaxContentLength = 3000;
    }
    
    private DirectMessage()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? Sender { get; init; }
    public Guid? SenderId { get; init; }
    
    public Conversation? Conversation { get; init; }
    public Guid? ConversationId { get; init; }
    public GroupConversation? GroupConversation { get; init; }
    public Guid? GroupConversationId { get; init; }
    
    public DirectMessage? ParentMessage { get; init; }
    public Guid? ParentMessageId { get; init; }
    public string? Content { get; private set; }
    public DateTime SentAt { get; init; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public bool IsReply { get; init; }
    
    private readonly List<DirectMessageRead> _reads = [];
    public IReadOnlyCollection<DirectMessageRead> Reads => _reads.AsReadOnly();

    /// <summary>Creates a new direct message instance for a two-member conversation.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<DirectMessage> ForConversation(User sender, Conversation conversation, string content,
        DirectMessage? parentMessage = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result<DirectMessage>.Error("Content cannot be empty.", ErrorType.ContentEmpty);
        
        if (content.Length > Limits.MaxContentLength)
            return Result<DirectMessage>
                .Error($"Direct message content cannot exceed {Limits.MaxContentLength} characters.",
                    ErrorType.ContentTooLong);
        
        return Result<DirectMessage>.Success(new DirectMessage
        {
            Sender = sender,
            SenderId = sender.Id,
            Conversation = conversation,
            ConversationId = conversation.Id,
            ParentMessage = parentMessage,
            ParentMessageId = parentMessage?.Id,
            IsReply = parentMessage is not null,
            Content = content
        });
    }

    /// <summary>Creates a new direct message for group chats.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<DirectMessage> ForGroupConversation(User sender, GroupConversation groupConversation,
        string content, DirectMessage? parentMessage = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result<DirectMessage>.Error("Content cannot be empty.", ErrorType.ContentEmpty);
        
        if (content.Length > Limits.MaxContentLength)
            return Result<DirectMessage>
                .Error($"Direct message content cannot exceed {Limits.MaxContentLength} characters.",
                    ErrorType.ContentTooLong);

        return Result<DirectMessage>.Success(new DirectMessage
        {
            Sender = sender,
            SenderId = sender.Id,
            GroupConversation = groupConversation,
            GroupConversationId = groupConversation.Id,
            ParentMessage = parentMessage,
            ParentMessageId = parentMessage?.Id,
            IsReply = parentMessage is not null,
            Content = content
        });
    }

    /// <summary>Edits an existing direct message.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyDeleted"/></item>
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string newContent)
    {
        if (IsDeleted)
            return Result.Error("Cannot edit a deleted message.", ErrorType.AlreadyDeleted);

        if (string.IsNullOrWhiteSpace(newContent))
            return Result.Error("Content cannot be empty.", ErrorType.ContentEmpty);
        
        if (newContent.Length > Limits.MaxContentLength)
            return Result.Error($"Direct message content cannot exceed {Limits.MaxContentLength} characters.",
                ErrorType.ContentTooLong);

        Content = newContent;
        EditedAt = DateTime.UtcNow;

        return Result.Success();
    }
    
    /// <summary>Soft deletes an existing direct message.</summary>
    /// <remarks>
    /// To be used instead of hard deleting a direct message in the database to preserve chat integrity.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyDeleted"/></item>
    /// </list>
    /// </remarks>
    public Result SoftDelete()
    {
        if (IsDeleted)
            return Result.Error("Message already deleted.", ErrorType.AlreadyDeleted);
        
        IsDeleted = true;
        Content = null;
        DeletedAt = DateTime.UtcNow;
        
        return Result.Success();
    }

    public void MarkAsReadBy(User user)
    {
        if (_reads.Any(r => r.UserId == user.Id))
            return;
        
        _reads.Add(new DirectMessageRead
        {
            DirectMessageId = Id,
            User = user,
            UserId = user.Id,
        });
    }
    
    public bool IsReadBy(Guid userId) => _reads.Any(r => r.UserId == userId);
}

public sealed class DirectMessageRead
{
    internal DirectMessageRead()
    {
    }

    public DirectMessage? DirectMessage { get; init; }
    public Guid DirectMessageId { get; init; }
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime ReadAt { get; init; } = DateTime.UtcNow;
}