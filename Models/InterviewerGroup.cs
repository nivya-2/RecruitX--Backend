using System.Collections.Generic;

namespace RecruitX.Models;

public class InterviewerGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<PanelToGroup> PanelMembers { get; set; } = new List<PanelToGroup>();
}
