using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecruitX.Models.DTO
{
    public class EvaluationFormPocDto
    {
        public string CandidateName { get; set; }
        public string JobRole { get; set; }
        public string InterviewLevel { get; set; }
        public string InterviewerPrompt { get; set; }

        public CandidateSummaryDto Summary { get; set; }

        // This list maps directly to the 'skills' FormArray in the Angular form
        public List<SkillBlockDto> Skills { get; set; }
        public string ProposedRole { get; set; }


        // e.g., "Please provide your feedback as the Technical Interviewer"
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
        [JsonPropertyName("feedbackJson")]

        public string FeedbackJson { get; set; } // The raw JSON feedback
    }
    public class SkillBlockDto
    {
        public string Category { get; set; }
        public List<CompetencyDto> Competencies { get; set; }
    }
    public class CompetencyDto
    {
        public string Title { get; set; }
        public int SelfRating { get; set; }
    }

    public class CandidateSummaryDto
    {
        public string CandidateName { get; set; }
        public string Technology { get; set; }
        public string InterviewLevel { get; set; }
        public string NoticePeriod { get; set; }
        public string TotalExperience { get; set; }
        public string RelevantExperience { get; set; }
        public string CurrentLocation { get; set; }
        public string PreferredLocation { get; set; }
    }

}
