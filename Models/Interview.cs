using System;
using RecruitX.Data;

namespace RecruitX.Models;

public class Interview
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }
    public Application Application { get; set; } = default!;

    public List<InterviewPanel> InterviewPanels { get; set; } = new();

    public Boolean IsTechnicalRound { get; set; }

    public DateTime ScheduledAt { get; set; }
    public DateTime ScheduledTo { get; set; }

    public InterviewStatus Status { get; set; }

    public string? EvaluationDetails { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
    public int? CreatedBy { get; set; }
    public User? Creator { get; set; }
    
}
