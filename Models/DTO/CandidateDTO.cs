namespace RecruitX.Models.DTO
{
    public class CandidateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public long MobNumber { get; set; }
        public string Email { get; set; } = null!;
        public string CurrentEmployer { get; set; } = null!;
        public string TotalExp { get; set; } = null!;     
        public string RelevantExp { get; set; } = null!;

        public string Stage { get; set; } = null!;
    }
}
