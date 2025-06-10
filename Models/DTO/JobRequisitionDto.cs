namespace RecruitX.DTOs;

public class JobRequisitionDto
{
    public int Id { get; init; }                        // Requisition ID

    public string Role { get; init; } = string.Empty;   // Job Title

    public string? DepartmentName { get; init; }        // Delivery Unit

    public string? LocationName { get; init; }          // Location

    public string? HiringManagerName { get; init; }     // Hiring Manager

    //public DateOnly? RequestedOn { get; init; }         // Requested On (was Uploaded On)
    public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    public bool IsAssigned { get; init; }               // Assignment flag
}
