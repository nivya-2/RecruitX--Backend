namespace RecruitX.Models
{
    public class LeadToRecruiter
    {
        public int Id { get; set; }
        public int RecruiterId { get; set; }

        public User Lead { get; set; }
        public User Recruiter { get; set; }
    }
}
