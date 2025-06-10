namespace RecruitX.Models.DTO
{
    public class AssignedJrDTO
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string DU { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public object JrProgress { get; set; }
        public string HiringManager { get; set; }
        public string AssignedOn { get; set; }
        public string CloseBy { get; set; }
    }
}
