using Threddit.Contracts.Common;

namespace Threddit.Contracts.Requests.Reports;

/// <summary>
/// Valid statuses: "Pending", "Received", "Dismissed" and "Resolved".
/// </summary>
public sealed record SetReportStatusApiRequest(string NewStatus);