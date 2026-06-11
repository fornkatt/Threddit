namespace Threddit.Infrastructure.Persistence.Repositories.Common;

internal static class RepositoryMessageConstants
{
    internal const string DatabaseTimeoutErrorMessage = "Database connection timed out or operation cancelled.";
    internal const string ConcurrencyFailureErrorMessage = "Failed to save to database due to concurrency issue.";
}