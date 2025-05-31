namespace RecruitX.Models;

public class PanelToGroup
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public InterviewerGroup Group { get; set; } = default!;

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
}
