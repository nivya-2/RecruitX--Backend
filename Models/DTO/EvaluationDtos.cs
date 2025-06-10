using System.ComponentModel.DataAnnotations;

namespace RecruitX.Models.DTO
{
    public class EvaluationFormPocDto
    {
        public string CandidateName { get; set; }
        public string JobRole { get; set; }
        public string InterviewLevel { get; set; }
        public string InterviewerPrompt { get; set; } // e.g., "Please provide your feedback as the Technical Interviewer"
    }

    /// <summary>
    /// DTO used when an interviewer submits their feedback.
    /// This is the request body for the POST /api/evaluation/submit endpoint.
    /// </summary>
    public class SubmitEvaluationDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string SubmittedByEmail { get; set; }

        [Required]
        public string FeedbackJson { get; set; } // The entire form data as a JSON string
    }

    /// <summary>
    // DTO for recruiters to view a submitted evaluation.
    /// </summary>
    public class ViewEvaluationDto
    {
        public string CandidateName { get; set; }
        public string JobRole { get; set; }
        public string InterviewLevel { get; set; }
        public string SubmittedByEmail { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string FeedbackJson { get; set; } // The raw JSON feedback
    }
}
