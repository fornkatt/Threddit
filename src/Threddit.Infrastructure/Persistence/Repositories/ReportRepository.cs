using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;
using Threddit.Infrastructure.Persistence.Repositories.Common;

namespace Threddit.Infrastructure.Persistence.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly ThredditDbContext _context;

    public ReportRepository(
        ThredditDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<Report>> GetByIdAsync(Guid id)
    {
        try
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id);
            return report is null
                ? Result<Report>.Error($"Report not found with ID {id}", ErrorType.ReportNotFound)
                : Result<Report>.Success(report);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Report>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<Report>> CreateAsync(Report report)
    {
        try
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return Result<Report>.Success(report);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Report>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result<Report>.Error("Database error while saving report.",
                ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> UpdateAsync(Report report)
    {
        try
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while updating report.",
                ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<ImmutableList<Report>>> GetBySubThreadAsync(Guid subThreadId,
        Report.ReportStatus? status = null, int page = 1, int pageSize = 20)
    {
        try
        {
            var query = _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.SubThread)
                .Include(r => r.TargetPost)
                .Include(r => r.TargetComment)
                .Where(r => r.SubThreadId == subThreadId);
            
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            
            var reports = await query
                .OrderByDescending(r => r.ReportedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return Result<ImmutableList<Report>>.Success([..reports]);
        }
        catch (OperationCanceledException ex)
        {
            return Result<ImmutableList<Report>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<ImmutableList<Report>>> GetSiteWideAsync(Report.ReportStatus? status = null,
        int page = 1, int pageSize = 20)
    {
        try
        {
            var query = _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.TargetUser)
                .Include(r => r.TargetSubThread)
                .Include(r => r.TargetDirectMessage)
                .Where(r => r.SubThreadId == null);
            
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            
            var reports = await query
                .OrderByDescending(r => r.ReportedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return Result<ImmutableList<Report>>.Success([..reports]);
        }
        catch (OperationCanceledException ex)
        {
            return Result<ImmutableList<Report>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }
}