namespace RecruitX.Models.DTO
{
    public class InterviewDTO
    {
        public string CandidateName { get; set; }
        public string JobRole { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string InterviewRound { get; set; }
        public string InterviewerName { get; set; }
        public int JobDescription { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

