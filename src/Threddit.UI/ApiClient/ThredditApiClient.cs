using System.Net.Http.Headers;
using System.Net.Http.Json;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.Auth;
using Threddit.Contracts.Requests.Comments;
using Threddit.Contracts.Requests.Conversations;
using Threddit.Contracts.Requests.Posts;
using Threddit.Contracts.Requests.Reports;
using Threddit.Contracts.Requests.SubThreads;
using Threddit.Contracts.Requests.Users;
using Threddit.Contracts.Responses.Auth;
using Threddit.Contracts.Responses.Comments;
using Threddit.Contracts.Responses.Conversations;
using Threddit.Contracts.Responses.Images;
using Threddit.Contracts.Responses.Posts;
using Threddit.Contracts.Responses.Reports;
using Threddit.Contracts.Responses.SubThreads;
using Threddit.Contracts.Responses.Users;
using Threddit.UI.ApiClient.Results;

namespace Threddit.UI.ApiClient;

public class ThredditApiClient
{
    private readonly HttpClient _client;

    public ThredditApiClient(
        HttpClient client
    )
    {
        _client = client;
    }

    /* AUTH ===================================================================================================== */
    public async Task<Result<LoginApiResponse>> LoginAsync(LoginApiRequest request, CancellationToken ct = default)
    {
        using var response = await _client.PostAsJsonAsync("api/auth/login", request, ct);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<LoginApiResponse>(ct);
            return Result<LoginApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<LoginApiResponse>.Error(error);
    }

    public async Task<Result<bool>> RegisterAsync(RegistrationApiRequest request, CancellationToken ct = default)
    {
        using var response = await _client.PostAsJsonAsync("api/auth/register", request, ct);
        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        try
        {
            await _client.PostAsync("api/auth/logout", null, ct);
        }
        catch
        {
            /* Catch unexpected errors during logout to not crash the auth flow. */
        }
    }

    /* SUBTHREAD ================================================================================================ */
    public async Task<Result<PagedApiResponse<GetSubThreadSearchApiResponse>>> SearchAsync(
        GetSubThreadsBySearchApiRequest request,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/search?query=" +
                  $"{Uri.EscapeDataString(request.Query)}&page={request.Page}&pageSize={request.PageSize}";
        using var response = await _client.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<PagedApiResponse<GetSubThreadSearchApiResponse>>(ct);
            return Result<PagedApiResponse<GetSubThreadSearchApiResponse>>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<PagedApiResponse<GetSubThreadSearchApiResponse>>.Error(error);
    }

    public async Task<Result<CreateSubThreadApiResponse>> CreateSubThreadAsync(CreateSubThreadApiRequest request,
        CancellationToken ct = default)
    {
        using var response = await _client.PostAsJsonAsync("api/subthreads", request, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<CreateSubThreadApiResponse>(ct);
            return Result<CreateSubThreadApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<CreateSubThreadApiResponse>.Error(error);
    }

    public async Task<Result<GetSubThreadApiResponse>> GetSubThreadByName(string subThreadName,
        CancellationToken ct = default)
    {
        using var response = await _client.GetAsync($"api/subthreads/{Uri.EscapeDataString(subThreadName)}", ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetSubThreadApiResponse>(ct);
            return Result<GetSubThreadApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetSubThreadApiResponse>.Error(error);
    }

    public async Task<Result<PagedApiResponse<GetSubThreadPostApiResponse>>> GetSubThreadPostsAsync(
        string subThreadName,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts?page={page}&pageSize={pageSize}";
        using var response = await _client.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<PagedApiResponse<GetSubThreadPostApiResponse>>(ct);
            return Result<PagedApiResponse<GetSubThreadPostApiResponse>>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<PagedApiResponse<GetSubThreadPostApiResponse>>.Error(error);
    }

    public async Task<Result<bool>> SubscribeToSubThreadAsync(string subThreadName, bool isSubscribed,
        CancellationToken ct = default)
    {
        HttpResponseMessage response;
        if (!isSubscribed)
        {
            var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/subscribe";
            response = await _client.PostAsync(url, null, ct);
        }
        else
        {
            var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/subscribe";
            response = await _client.DeleteAsync(url, ct);
        }

        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    /* POST ===================================================================================================== */
    public async Task<Result<CreatePostApiResponse>> CreatePostAsync(
        string subThreadName,
        CreatePostApiRequest request,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts";
        using var response = await _client.PostAsJsonAsync(url, request, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<CreatePostApiResponse>(ct);
            return Result<CreatePostApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<CreatePostApiResponse>.Error(error);
    }

    public async Task<Result<GetPostWithCommentsApiResponse>> GetPostWithCommentsAsync(
        string subThreadName,
        Guid postId,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}";
        using var response = await _client.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetPostWithCommentsApiResponse>(ct);
            return Result<GetPostWithCommentsApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetPostWithCommentsApiResponse>.Error(error);
    }

    public async Task<Result<bool>> EditPostAsync(
        string subThreadName,
        Guid postId,
        EditPostApiRequest request,
        CancellationToken ct = default)
    {
        using var response = await _client.PatchAsJsonAsync(
            $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}", request, ct);

        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    public async Task<Result<bool>> DeletePostAsync(
        string subThreadName,
        Guid postId,
        string? reason = null,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}";
        if (!string.IsNullOrWhiteSpace(reason))
            url += $"?reason={Uri.EscapeDataString(reason)}";

        using var response = await _client.DeleteAsync(url, ct);

        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    /* COMMENT ================================================================================================== */
    public async Task<Result<CreateCommentApiResponse>> CreateCommentAsync(
        string subThreadName,
        Guid postId,
        CreateCommentApiRequest request,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}/comments";
        using var response = await _client.PostAsJsonAsync(url, request, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<CreateCommentApiResponse>(ct);
            return Result<CreateCommentApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<CreateCommentApiResponse>.Error(error);
    }

    public async Task<Result<IReadOnlyList<GetCommentApiResponse>>> GetCommentRepliesAsync(
        string subThreadName,
        Guid postId,
        Guid commentId,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}/comments/{commentId}/replies";
        using var response = await _client.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetCommentApiResponse>>(ct);
            return Result<IReadOnlyList<GetCommentApiResponse>>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<IReadOnlyList<GetCommentApiResponse>>.Error(error);
    }

    public async Task<Result<bool>> EditCommentAsync(
        string subThreadName,
        Guid postId,
        Guid commentId,
        EditCommentApiRequest request,
        CancellationToken ct = default)
    {
        using var response = await _client.PatchAsJsonAsync(
            $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}/comments/{commentId}",
            request, ct);

        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    public async Task<Result<bool>> DeleteCommentAsync(
        string subThreadName,
        Guid postId,
        Guid commentId,
        string? reason = null,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}/comments/{commentId}";
        if (!string.IsNullOrWhiteSpace(reason))
            url += $"?reason={Uri.EscapeDataString(reason)}";

        using var response = await _client.DeleteAsync(url, ct);

        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    /* USER ==================================================================================================== */
    public async Task<Result<GetCurrentUserSummaryApiResponse>> GetCurrentUserSummaryAsync(
        CancellationToken ct = default)
    {
        using var response = await _client.GetAsync("api/users/me/summary", ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetCurrentUserSummaryApiResponse>(ct);
            return Result<GetCurrentUserSummaryApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetCurrentUserSummaryApiResponse>.Error(error);
    }

    public async Task<Result<GetUserProfileApiResponse>> GetUserProfileAsync(string username,
        CancellationToken ct = default)
    {
        using var response = await _client.GetAsync($"api/users/{Uri.EscapeDataString(username)}", ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetUserProfileApiResponse>(ct);
            return Result<GetUserProfileApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetUserProfileApiResponse>.Error(error);
    }

    public async Task<Result<bool>> EditUserProfileAsync(EditUserProfileApiRequest request,
        CancellationToken ct = default)
    {
        using var response = await _client.PatchAsJsonAsync("api/users/me", request, ct);

        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    /* REPORTS ================================================================================================== */
    public async Task<Result<CreateReportApiResponse>> CreateSubThreadReportAsync(
        string subThreadName,
        CreateSubThreadReportApiRequest request,
        CancellationToken ct = default)
    {
        var url = $"api/moderation/subthreads/{Uri.EscapeDataString(subThreadName)}/reports";
        using var response = await _client.PostAsJsonAsync(url, request, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<CreateReportApiResponse>(ct);
            return Result<CreateReportApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<CreateReportApiResponse>.Error(error);
    }

    public async Task<Result<GetReportsApiResponse>> GetSubThreadReportsAsync(
        string subThreadName,
        string? status = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var url =
            $"api/moderation/subthreads/{Uri.EscapeDataString(subThreadName)}/reports?page={page}&pageSize={pageSize}";
        if (status is not null)
            url += $"&status={Uri.EscapeDataString(status)}";

        using var response = await _client.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetReportsApiResponse>(ct);
            return Result<GetReportsApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetReportsApiResponse>.Error(error);
    }

    public async Task<Result<GetReportsApiResponse>> GetSiteReportsAsync(
        string? status = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var url = $"api/moderation/reports?page={page}&pageSize={pageSize}";
        if (status is not null)
            url += $"&status={Uri.EscapeDataString(status)}";

        using var response = await _client.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetReportsApiResponse>(ct);
            return Result<GetReportsApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetReportsApiResponse>.Error(error);
    }

    public async Task<Result<bool>> SetReportStatusAsync(
        Guid reportId,
        SetReportStatusApiRequest request,
        CancellationToken ct = default)
    {
        var url = $"api/moderation/reports/{reportId}/status";
        using var response = await _client.PatchAsJsonAsync(url, request, ct);

        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    /* IMAGES =================================================================================================== */
    public async Task<Result<string>> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        string category,
        CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);

        using var response = await _client
            .PostAsync($"api/images/upload?category={Uri.EscapeDataString(category)}", content, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<UploadImageApiResponse>(ct);
            return Result<string>.Ok(body!.Url);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<string>.Error(error);
    }

    /* VOTES ==================================================================================================== */
    public async Task<Result<int>> VotePostAsync(
        string subThreadName,
        Guid postId,
        bool isUpvote,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}/posts/{postId}/vote?isUpvote={isUpvote}";
        using var repsonse = await _client
            .PostAsync(url, null, ct);

        if (repsonse.IsSuccessStatusCode)
        {
            var body = await repsonse.Content.ReadFromJsonAsync<VotePostApiResponse>(ct);
            return Result<int>.Ok(body!.NewScore);
        }

        var error = await TryReadErrorMessage(repsonse, ct);
        return Result<int>.Error(error);
    }

    public async Task<Result<int>> VoteCommentAsync(
        string subThreadName,
        Guid postId,
        Guid commentId,
        bool isUpvote,
        CancellationToken ct = default)
    {
        var url = $"api/subthreads/{Uri.EscapeDataString(subThreadName)}" +
                  $"/posts/{postId}/comments/{commentId}/vote?isUpvote={isUpvote}";
        using var response = await _client.PostAsync(url, null, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<VoteCommentApiResponse>(ct);
            return Result<int>.Ok(body!.NewScore);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<int>.Error(error);
    }

    /* CONVERSATIONS =========================================================================================== */
    public async Task<Result<GetConversationsApiResponse>> GetConversationsAsync(CancellationToken ct = default)
    {
        using var response = await _client.GetAsync("api/conversations", ct);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetConversationsApiResponse>(ct);
            return Result<GetConversationsApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetConversationsApiResponse>.Error(error);
    }

    public async Task<Result<Guid>> StartConversationAsync(string username, CancellationToken ct = default)
    {
        using var response =
            await _client.PostAsync($"api/conversations/with/{Uri.EscapeDataString(username)}",
                null, ct);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<CreateConversationApiResponse>(ct);
            return Result<Guid>.Ok(body!.ConversationId);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<Guid>.Error(error);
    }

    public async Task<Result<GetMessagesApiResponse>> GetConversationMessagesAsync(
        Guid conversationId,
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        using var response = await _client
            .GetAsync($"api/conversations/{conversationId}/messages?page={page}&pageSize={pageSize}", ct);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetMessagesApiResponse>(ct);
            return Result<GetMessagesApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetMessagesApiResponse>.Error(error);
    }

    public async Task<Result<DirectMessageApiDto>> SendMessageAsync(
        Guid conversationId,
        SendMessageApiRequest apiRequest,
        CancellationToken ct = default)
    {
        using var response = await _client
            .PostAsJsonAsync($"api/conversations/{conversationId}/messages", apiRequest, ct);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<SendMessageApiResponse>(ct);
            return Result<DirectMessageApiDto>.Ok(body!.Message);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<DirectMessageApiDto>.Error(error);
    }

    public async Task<Result<Guid>> CreateGroupConversationAsync(
        CreateGroupConversationApiRequest apiRequest,
        CancellationToken ct = default)
    {
        using var response = await _client.PostAsJsonAsync("api/conversations/groups", apiRequest, ct);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<CreateGroupConversationApiResponse>(ct);
            return Result<Guid>.Ok(body!.GroupConversationId);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<Guid>.Error(error);
    }

    public async Task<Result<GetMessagesApiResponse>> GetGroupMessagesAsync(
        Guid groupId,
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        using var response = await _client
            .GetAsync($"api/conversations/groups/{groupId}/messages?page={page}&pageSize={pageSize}", ct);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<GetMessagesApiResponse>(ct);
            return Result<GetMessagesApiResponse>.Ok(body!);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<GetMessagesApiResponse>.Error(error);
    }

    public async Task<Result<DirectMessageApiDto>> SendGroupMessageAsync(
        Guid groupId,
        SendMessageApiRequest apiRequest,
        CancellationToken ct = default)
    {
        using var response = await _client
            .PostAsJsonAsync($"api/conversations/groups/{groupId}/messages", apiRequest, ct);
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<SendMessageApiResponse>(ct);
            return Result<DirectMessageApiDto>.Ok(body!.Message);
        }

        var error = await TryReadErrorMessage(response, ct);
        return Result<DirectMessageApiDto>.Error(error);
    }

    public async Task<Result<bool>> AddGroupMemberAsync(
        Guid groupId,
        AddGroupMemberApiRequest apiRequest,
        CancellationToken ct = default)
    {
        using var response = await _client
            .PostAsJsonAsync($"api/conversations/groups/{groupId}/members", apiRequest, ct);
        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    public async Task<Result<bool>> LeaveGroupAsync(
        Guid groupId,
        CancellationToken ct = default)
    {
        using var response = await _client
            .DeleteAsync($"api/conversations/groups/{groupId}/members/me", ct);
        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }

    public async Task<Result<bool>> RemoveGroupMemberAsync(
        Guid groupId,
        Guid targetUserId,
        CancellationToken ct = default)
    {
        using var response = await _client
            .DeleteAsync($"api/conversations/groups/{groupId}/members/{targetUserId}", ct);
        if (response.IsSuccessStatusCode)
            return Result<bool>.Ok(true);

        var error = await TryReadErrorMessage(response, ct);
        return Result<bool>.Error(error);
    }
    
    private static async Task<string?> TryReadErrorMessage(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);

        if (string.IsNullOrWhiteSpace(content))
            return $"Request failed with status {(int)response.StatusCode}.";

        try
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var error = System.Text.Json.JsonSerializer.Deserialize<ErrorResponse>(content, options);
            return error?.Message ?? $"Request failed with status {(int)response.StatusCode}.";
        }
        catch
        {
            return $"Request failed with status {(int)response.StatusCode}.";
        }
    }
}