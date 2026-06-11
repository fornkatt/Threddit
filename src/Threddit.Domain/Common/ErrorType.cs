namespace Threddit.Domain.Common;

/// <summary>
/// A collection of error types that can occur in the whole application.
/// </summary>
public enum ErrorType
{
    Unknown,
    
    // Not Found
    NotFound,
    PostNotFound,
    CommentNotFound,
    SubThreadNotFound,
    UserNotFound,
    ReportNotFound,
    SiteAdminNotFound,
    
    // Validation — Content
    TitleEmpty,
    TitleTooLong,
    NameEmpty,
    NameTooLong,
    ContentEmpty,
    ContentTooLong,
    DeleteReasonTooLong,
    ImageUrlTooLong,
    CreatorNameEmpty,
    BanReasonTooLong,
    
    // Validation — User
    InvalidEmail,
    EmailTooLong,
    InvalidUsername,
    UsernameTooLong,
    InvalidBanDate,
    BanReasonEmpty,
    
    // Validation - SubThread
    PostDoesNotBelongToSubThread,
    SubThreadRuleNotFound,
    
    // Validation - Post
    CommentDoesNotBelongToPost,
    
    // Validation - Report
    InvalidReportType,
    SameStatus,
    
    // Validation - Conversation
    ConversationNotFound,
    NotAMember,
    
    // Validation - DirectMessage
    DirectMessageNotFound,
    
    // Conflict
    AlreadyDeleted,
    AlreadyAssigned,
    AlreadyMember,
    AlreadySubscribed,
    NotSubscribed,
    
    // Auth
    InvalidCredentials,
    EmailTaken,
    UsernameTaken,
    RegistrationFailed,
    
    // Authorization
    Forbidden,
    SubThreadBanned,
    DeleteReasonRequired,
    
    // Database
    DatabaseUpdateFailure,
    ConcurrencyFailure,
    DatabaseTimeout
}