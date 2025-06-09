// File Path: Models/DTO/InterviewPanelDtos.cs

using System.ComponentModel.DataAnnotations;

namespace RecruitX.Models.DTO
{
    // --- DTO for listing panel members with details ---
    public class PanelMemberDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public bool IsActiveInAD { get; set; }
    }
    // File Path: Models/DTO/InterviewPanelDtos.cs

    // ... (existing DTOs) ...

    // ADD THIS NEW DTO for the panel members list
    public class PanelMemberNameAndEmailDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    // --- DTO for getting interview meeting details ---
    public class InterviewMeetingDetailsDto
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string CandidateName { get; set; }
        public string Position { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string TimeZone { get; set; }
        public string TeamsJoinUrl { get; set; }
        public string Organizer { get; set; }
        public List<MeetingAttendeeDto> Attendees { get; set; }
    }

    public class MeetingAttendeeDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    // --- DTOs for scheduling a new interview ---
    public class ScheduleInterviewRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one panel member email is required.")]
        [MaxLength(3, ErrorMessage = "You can select a maximum of 3 panel members.")]
        public List<string> PanelMemberEmails { get; set; }

        // --- MODIFIED PROPERTIES ---
        [Required(ErrorMessage = "Date is required.")]
        [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "Date must be in dd-MM-yyyy format.")]
        public string Date { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public string EndTime { get; set; }
        // --- END OF MODIFIED PROPERTIES ---

        [Required]
        [StringLength(100)]
        public string CandidateName { get; set; }

        [Required]
        [StringLength(100)]
        public string InterviewLevel { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Job Role cannot exceed 100 characters.")]
        public string JobRole { get; set; }
    }

    public class ScheduleInterviewResponseDto
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string TeamsJoinUrl { get; set; }
        public string WebLink { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

} // <--- This is now the final closing brace for the namespace.