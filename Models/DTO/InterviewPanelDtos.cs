using System.ComponentModel.DataAnnotations;

namespace RecruitX.Models.DTO
{
    public class PanelMemberNameAndEmailDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// DTO for the GET /panel-meetings endpoint.
    /// Represents a single meeting block to be displayed on the grid.
    /// </summary>
    public class InterviewMeetingDetailsDto
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string CandidateName { get; set; }
        public string StartTime { get; set; } // Will be in 'HH:mm' 24-hour format
        public string EndTime { get; set; }   // Will be in 'HH:mm' 24-hour format
        public List<string> InterviewerEmails { get; set; }
    }

    /// <summary>
    /// DTO for the POST /schedule-interview request body.
    /// This captures all the information from the frontend form.
    /// </summary>
    public class ScheduleInterviewRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one panel member must be selected.")]
        public List<string> PanelMemberEmails { get; set; }

        [Required]
        public string Date { get; set; } // Expected format: "dd-MM-yyyy"

        [Required]
        public string StartTime { get; set; } // Expected format: "h:mm tt" (e.g., "9:30 AM")

        [Required]
        public string EndTime { get; set; }

        [Required]
        public string CandidateName { get; set; }

        [Required]
        public string InterviewLevel { get; set; }

        [Required]
        public string JobRole { get; set; }
    }

    /// <summary>
    /// DTO for the POST /schedule-interview successful response.
    /// Returns key details of the newly created meeting.
    /// </summary>
    public class ScheduleInterviewResponseDto
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string TeamsJoinUrl { get; set; }
        public string WebLink { get; set; }
        // This was added in the last step to return the one-time link
        public string EvaluationLink { get; set; }
    }
}
