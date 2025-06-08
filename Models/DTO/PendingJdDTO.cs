using RecruitX.Data;

namespace RecruitX.Models.DTO
{
    public class PendingJdDTO
    {
        public int JobRequisitionId { get; set; }
        public string RoleTitle { get; set; }
        public string BusinessUnit { get; set; }
        public string location {  get; set; }

        public int openPositions {  get; set; }

        public string HiringManager {  get; set; }
        public DateOnly CreatedDate { get; set; }

        public JobStatus JobStatus { get; set; }

        public List<string> Actions { get; set; }

        public PendingJdDTO()
        {
            Actions = new List<string> { "Generate JD" };
        }
    }
}
