namespace RecruitX.Models.DTO
{
    public class TrackJobRequisitionDTO
    {
        public int Id { get; set; }                        // Requisition ID

        public string Role { get; set; } = string.Empty;   // Job Title

        public string? DepartmentName { get; set; }        // Delivery Unit

        public string? LocationName { get; set; }          // Location

        public string? HiringManagerName { get; set; }     // Hiring Manager

       public string status { get; set; }

        public int NumPositions {  get; set; }
        public int FilledPositions {  get; set; }

        public string assignedTo {  get; set; }
        public DateOnly assignedOn { get; set; }
        public DateOnly CloseBy { get; set; }
    }
}
