namespace RecruitX.Models.DTO
{
    public class ToShortlistDto
    {
        public int InterviewId { get; set; }
        public string Id { get; set; }  // Candidate ID or custom code
        public int JdId { get; set; }
        public string Name { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string InterviewType { get; set; }
        public List<string> Actions { get; set; }
    }
}

