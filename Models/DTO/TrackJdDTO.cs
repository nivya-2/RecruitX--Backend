using RecruitX.Data;

namespace RecruitX.Models.DTO
{
    public class TrackJdDTO
    {
        public int JobRequisitionId {  get; set; }
        public string RoleTitle {  get; set; }
        public string BusinessUnit {  get; set; }

        public DateOnly CreatedDate { get; set; }

        public JobStatus JobStatus { get; set; }

        public List<string> Actions { get; set; }

        public TrackJdDTO()
        {
            Actions = new List<string> { "View JD", "View Applicants" }; 
        }
    }
}
