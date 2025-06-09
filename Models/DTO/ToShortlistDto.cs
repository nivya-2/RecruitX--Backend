namespace RecruitX.Models.DTO
{
    public class ToShortlistDto
    {
        public string Id { get; set; }  // Candidate ID or custom code
        public string Name { get; set; }
        public string InterviewDate { get; set; }
        public string InterviewType { get; set; }
        public List<string> Actions { get; set; }
    }
}
