namespace Threddit.Contracts.Responses.Validation;

public sealed record ValidationLimitsApiResponse(
    int PostMaxTitleLength,
    int PostMaxSlugLength,
    int PostMaxContentLength,
    int PostMaxImageUrlLength,
    int PostMaxDeleteReasonLength,
    
    int CommentMaxContentLength,
    int CommentMaxDeleteReasonLength,
    
    int SubThreadMaxNameLength,
    int SubThreadMaxDescriptionLength,
    int SubThreadMaxBannerImageUrlLength,
    
    int SubThreadRuleMaxTitleLength,
    int SubThreadRuleMaxContentLength,
    
    int UserMaxUsernameLength,
    int UserMaxEmailLength,
    int UserMaxProfilePictureUrlLength,
    int UserMaxDescriptionLength,
    
    int GroupConversationMaxNameLength,
    
    int DirectMessageMaxContentLength,
    
    int ReportMaxMessageLength
);