namespace Threddit.UI.Features.Shared.Models;

public sealed class ReportStatusModel
{
    public Guid ReportId { get; set; }
    public string Status { get; set; } = "Pending";
}