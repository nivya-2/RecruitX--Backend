namespace RecruitX.Models.DTO
{
    public class AssignJrDTO
    {
        public int JobRequisitionId { get; set; }
        public int AssignedToUserId { get; set; }
        public string AssignedByUsername { get; set; } = string.Empty;
        public bool ForceReassign { get; set; } = false;
    }
}
