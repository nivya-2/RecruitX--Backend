namespace RecruitX.Models
{
    public sealed class Candidate
    {
        public int Id { get; set; }
        public string Source { get; set; } = null!;
        public string? SubSource { get; set; }
        public string CandidateName { get; set; } = null!;
        public string ProposedRole { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long ContactNumber { get; set; }
        public string? LinkedinUrl { get; set; }
        public short TotalExperienceYears { get; set; }
        public short TotalExperienceMonths { get; set; }
        public short RelevantExperienceYears { get; set; }
        public short RelevantExperienceMonths { get; set; }
        public string? CurrentEmployer { get; set; }

        public int? CurrentLocationId { get; set; }
        public Location? CurrentLocation { get; set; }

        public int? PreferredLocationId { get; set; }
        public Location? PreferredLocation { get; set; }

        public int? NoticePeriodDays { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }


}
