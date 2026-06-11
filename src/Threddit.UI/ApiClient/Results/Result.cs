namespace Threddit.UI.ApiClient.Results;

/// <summary>
/// Thin wrapper of the domain Result type to handle API responses with an optional message on non-success status codes.
/// </summary>
public sealed record Result(bool IsSuccess, string? ErrorMessage = null)
{
    public static Result Ok() => new(true);

    public static Result Error(string? errorMessage) => new(false, errorMessage);
}

/// <summary>
/// Thin wrapper of the domain Result type to handle API responses with an optional message on non-success status codes.
/// </summary>
public sealed record Result<T>(bool IsSuccess, T? Value = default, string? ErrorMessage = null)
{
    public static Result<T> Ok(T value) => new(true, value);

    public static Result<T> Error(string? errorMessage) => new(false, ErrorMessage: errorMessage);
}