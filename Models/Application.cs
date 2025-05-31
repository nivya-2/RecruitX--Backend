using RecruitX.Data;

namespace RecruitX.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int JobDescriptionId { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
        public DateTime? SubmittedOn { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }

        public short ExperienceYears { get; set; }
        public short ExperienceMonths { get; set; } // 0-11 validated in fluent API

        // Navigation
        public Candidate Candidate { get; set; } = null!;
        public JobDescription JobDescription { get; set; } = null!;
        public User? CreatedByUser { get; set; }

        public ICollection<ApplicationSkill> ApplicationSkills { get; set; } = new List<ApplicationSkill>();
    }




}
