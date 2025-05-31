using System;

namespace RecruitX.Models;

public class EmailTemplateVariable
{
    public int Id { get; set; }

    public int TemplateId { get; set; }
    public EmailTemplate Template { get; set; } = null!;

    public string VariableName { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

