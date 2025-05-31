
using RecruitX.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; } = false;

    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}

