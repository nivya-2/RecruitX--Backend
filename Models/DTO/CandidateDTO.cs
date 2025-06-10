namespace RecruitX.Models.DTO
{
    public class CandidateDTO
    {
        public int Id { get; set; }
        public string CandidateName { get; set; } = null!;
        public long MobileNumber { get; set; }
        public string Email { get; set; } = null!;
        public string CurrentEmployer { get; set; } = null!;
        public string TotalExperience { get; set; } = null!;     
        public string RelevantExperience { get; set; } = null!;

        public string Stage { get; set; } = null!;
    }
}
