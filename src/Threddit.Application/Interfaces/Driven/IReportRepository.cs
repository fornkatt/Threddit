using System.Collections.Immutable;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.Interfaces.Driven;

public interface IReportRepository
{
    /// <summary>Gets a single report by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ReportNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Report>> GetByIdAsync(Guid id);
    
    
    /// <summary>Saves a new report to the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Report>> CreateAsync(Report report);
    
    
    /// <summary>Saves changes to an existing report (e.g., status update).</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UpdateAsync(Report report);
    
    /// <summary>Gets paginated reports scoped to a SubThread (Post and Comment reports).</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<ImmutableList<Report>>> GetBySubThreadAsync(Guid subThreadId, Report.ReportStatus? status = null,
        int page = 1, int pageSize = 20);
    
    /// <summary>Gets paginated site-wide reports (User, SubThread, Direct Message reports).</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<ImmutableList<Report>>> GetSiteWideAsync(Report.ReportStatus? status = null,
        int page = 1, int pageSize = 20);
}