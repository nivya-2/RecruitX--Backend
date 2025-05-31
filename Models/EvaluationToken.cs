using System;

namespace RecruitX.Models;

public class EvaluationToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public int InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
