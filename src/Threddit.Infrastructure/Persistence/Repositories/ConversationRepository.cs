using Microsoft.EntityFrameworkCore;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;
using Threddit.Infrastructure.Persistence.Repositories.Common;

namespace Threddit.Infrastructure.Persistence.Repositories;

public sealed class ConversationRepository : IConversationRepository
{
    private readonly ThredditDbContext _context;

    public ConversationRepository(
        ThredditDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<DirectMessage>> GetByIdAsync(Guid id)
    {
        try
        {
            var directMessage = await _context.DirectMessages.FirstOrDefaultAsync(dm => dm.Id == id);

            return directMessage is null
                ? Result<DirectMessage>.Error($"Direct message not found with ID {id}", ErrorType.DirectMessageNotFound)
                : Result<DirectMessage>.Success(directMessage);
        }
        catch (OperationCanceledException ex)
        {
            return Result<DirectMessage>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<Conversation>> GetOrCreateConversationAsync(Guid userId, Guid otherUserId)
    {
        try
        {
            var (id1, id2) = userId.CompareTo(otherUserId) < 0
                ? (userId, otherUserId)
                : (otherUserId, userId);

            var existing = await _context.Conversations
                .FirstOrDefaultAsync(c => c.InitiatorId == id1 && c.RecipientId == id2);

            if (existing is not null)
            {
                existing.Unhide();
                await _context.SaveChangesAsync();
                return Result<Conversation>.Success(existing);
            }

            var initiator = await _context.Users.FirstOrDefaultAsync(u => u.Id == id1);
            var recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == id2);

            if (initiator is null || recipient is null)
                return Result<Conversation>.Error("User not found.", ErrorType.UserNotFound);

            var conversation = Conversation.Create(initiator, recipient);
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return Result<Conversation>.Success(conversation);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Conversation>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<List<Conversation>>> GetConversationsForUserAsync(Guid userId)
    {
        try
        {
            var conversations = await _context.Conversations
                .Include(c => c.Initiator)
                .Include(c => c.Recipient)
                .Where(c =>
                    (c.InitiatorId == userId && !c.InitiatorHidden) ||
                    (c.RecipientId == userId && !c.RecipientHidden))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Result<List<Conversation>>.Success(conversations);
        }
        catch (OperationCanceledException ex)
        {
            return Result<List<Conversation>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<List<DirectMessage>>> GetConversationMessagesAsync(Guid conversationId, int page,
        int pageSize)
    {
        try
        {
            var messages = await _context.DirectMessages
                .Include(m => m.Sender)
                .Include(m => m.ParentMessage)
                .ThenInclude(p => p!.Sender)
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<List<DirectMessage>>.Success(messages);
        }
        catch (OperationCanceledException ex)
        {
            return Result<List<DirectMessage>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<DirectMessage>> SendToConversationAsync(Guid conversationId, Guid senderId, string content,
        Guid? parentMessageId = null)
    {
        try
        {
            var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId);
            if (conversation is null)
                return Result<DirectMessage>.Error("Conversation not found", ErrorType.ConversationNotFound);

            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == senderId);
            if (sender is null)
                return Result<DirectMessage>.Error("User not found", ErrorType.UserNotFound);

            DirectMessage? parent = null;
            if (parentMessageId.HasValue)
                parent = await _context.DirectMessages
                    .FirstOrDefaultAsync(m => m.Id == parentMessageId.Value);

            var result = DirectMessage.ForConversation(sender, conversation, content, parent);
            if (!result.IsSuccess)
                return Result<DirectMessage>.Error(result.ErrorMessage, result.ErrorType);

            conversation.Unhide();
            _context.DirectMessages.Add(result.Value!);
            await _context.SaveChangesAsync();
            return Result<DirectMessage>.Success(result.Value!);
        }
        catch (OperationCanceledException ex)
        {
            return Result<DirectMessage>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<GroupConversation>> CreateGroupConversationAsync(Guid creatorId, string name,
        IEnumerable<Guid> memberIds)
    {
        try
        {
            var creator = await _context.Users.FirstOrDefaultAsync(u => u.Id == creatorId);
            if (creator is null)
                return Result<GroupConversation>.Error("User not found", ErrorType.UserNotFound);

            var memberIdList = memberIds.Distinct()
                .Where(id => id != creatorId)
                .ToList();
            var members = await _context.Users
                .Where(u => memberIdList.Contains(u.Id))
                .ToListAsync();

            var result = GroupConversation.Create(creator, name, members);
            if (!result.IsSuccess)
                return Result<GroupConversation>.Error(result.ErrorMessage, result.ErrorType);

            _context.GroupConversations.Add(result.Value!);
            await _context.SaveChangesAsync();
            return Result<GroupConversation>.Success(result.Value!);
        }
        catch (OperationCanceledException ex)
        {
            return Result<GroupConversation>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<List<GroupConversation>>> GetGroupConversationsForUserAsync(Guid userId)
    {
        try
        {
            var groups = await _context.GroupConversations
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .Where(g => g.Members.Any(m => m.UserId == userId && !m.LeftAt.HasValue))
                .ToListAsync();

            return Result<List<GroupConversation>>.Success(groups);
        }
        catch (OperationCanceledException ex)
        {
            return Result<List<GroupConversation>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<List<DirectMessage>>> GetGroupConversationMessagesAsync(Guid groupConversationId, int page,
        int pageSize)
    {
        try
        {
            var messages = await _context.DirectMessages
                .Include(m => m.Sender)
                .Include(m => m.ParentMessage)
                .ThenInclude(p => p!.Sender)
                .Where(m => m.GroupConversationId == groupConversationId)
                .OrderBy(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<List<DirectMessage>>.Success(messages);
        }
        catch (OperationCanceledException ex)
        {
            return Result<List<DirectMessage>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<DirectMessage>> SendToGroupConversationAsync(Guid groupConversationId, Guid senderId,
        string content,
        Guid? parentMessageId = null)
    {
        try
        {
            var group = await _context.GroupConversations
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupConversationId);
            if (group is null)
                return Result<DirectMessage>.Error("Group conversation not found", ErrorType.ConversationNotFound);

            if (!group.Members.Any(m => m.UserId == senderId && !m.LeftAt.HasValue))
                return Result<DirectMessage>.Error("User not a member of the group", ErrorType.NotAMember);

            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == senderId);
            if (sender is null)
                return Result<DirectMessage>.Error("User not found", ErrorType.UserNotFound);

            DirectMessage? parent = null;
            if (parentMessageId.HasValue)
                parent = await _context.DirectMessages
                    .FirstOrDefaultAsync(m => m.Id == parentMessageId.Value);

            var result = DirectMessage.ForGroupConversation(sender, group, content, parent);
            if (!result.IsSuccess)
                return Result<DirectMessage>.Error(result.ErrorMessage, result.ErrorType);

            _context.DirectMessages.Add(result.Value!);
            await _context.SaveChangesAsync();
            return Result<DirectMessage>.Success(result.Value!);
        }
        catch (OperationCanceledException ex)
        {
            return Result<DirectMessage>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result> AddMemberToGroupAsync(Guid groupConversationId, Guid requestingUserId, Guid newMemberId)
    {
        try
        {
            var group = await _context.GroupConversations
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupConversationId);
            if (group == null)
                return Result.Error("Group conversation not found", ErrorType.ConversationNotFound);
            
            if (!group.Members.Any(m => m.UserId == requestingUserId && !m.LeftAt.HasValue))
                return Result.Error("You are not a member of the group", ErrorType.NotAMember);

            var newUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == newMemberId);
            if (newUser == null)
                return Result.Error("User not found", ErrorType.UserNotFound);
            
            var addResult = group.AddMember(newUser);
            if (!addResult.IsSuccess)
                return Result.Error(addResult.ErrorMessage, addResult.ErrorType);
            
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
    }
}