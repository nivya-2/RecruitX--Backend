using RecruitX.Models.DTO;

namespace RecruitX.Controllers
{
    public class ApplicationDetailsDTO
    {
        public class ApplicationTimelineStepDto
        {
            public string Label { get; set; }
            public string? Date { get; set; } // Nullable, as pending steps have no date
            public bool Completed { get; set; }
        }

        // This is the new main DTO for your page. It contains the other DTOs.
        public class ApplicationDetailsPageDTO
        {
            // Contains the candidate info you already have
            public CandidateDetailsDTO CandidateInfo { get; set; }

            // Contains the new dynamic timeline
            public List<ApplicationTimelineStepDto> StatusTimeline { get; set; }

            // A helper property for the UI
            public bool IsProcessFinished { get; set; }
        }

        // DTO for the request body when updating status
        public class UpdateStatusRequestDto
        {
            public string Action { get; set; } // "progress" or "reject"
        }
    }
}
