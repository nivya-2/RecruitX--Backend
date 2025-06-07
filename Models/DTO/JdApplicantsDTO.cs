namespace RecruitX.Models.DTO
{
    public class JdApplicantsDTO
    {
        public int CandidateId {  get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public long CandidatePhone { get; set; }

        public short TotalExperienceYears { get; set; }

        public string Source {  get; set; }

        public List<string> Actions { get; set; }

        public JdApplicantsDTO()
        {
            Actions = new List<string> { "Details" };
        }

    }
}
