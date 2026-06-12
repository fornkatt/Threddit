using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.Interfaces.Driven;

public interface IConversationRepository
{
    /// <summary>Gets a single Direct Message by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DirectMessageNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<DirectMessage>> GetByIdAsync(Guid id);

    // One-to-one Conversations
    Task<Result<Conversation>> GetOrCreateConversationAsync(Guid userId, Guid otherUserId);
    Task<Result<List<Conversation>>> GetConversationsForUserAsync(Guid userId);
    Task<Result<List<DirectMessage>>> GetConversationMessagesAsync(Guid conversationId, int page, int pageSize);
    Task<Result<DirectMessage>> SendToConversationAsync(Guid conversationId, Guid senderId, string content,
        Guid? parentMessageId = null);

    // Group Conversations
    Task<Result<GroupConversation>> CreateGroupConversationAsync(Guid creatorId, string name,
        IEnumerable<Guid> memberIds);
    Task<Result<List<GroupConversation>>> GetGroupConversationsForUserAsync(Guid userId);
    Task<Result<List<DirectMessage>>> GetGroupConversationMessagesAsync(Guid groupConversationId, int page,
        int pageSize);
    Task<Result<DirectMessage>> SendToGroupConversationAsync(Guid groupConversationId, Guid senderId, string content,
        Guid? parentMessageId = null);
    Task<Result> AddMemberToGroupAsync(Guid groupConversationId, Guid requestingUserId, Guid newMemberId);
    
    /// <summary>Allows any member of a group to leave the conversation.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConversationNotFound"/></item>
    ///     <item><see cref="ErrorType.NotAMember"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> LeaveGroupAsync(Guid groupConversationId, Guid userId);
    
    /// <summary>Allows the creator of a group conversation to remove a member.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConversationNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.NotAMember"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> RemoveMemberFromGroupAsync(Guid groupConversationId, Guid requestingUserId, Guid targetUserId);
}