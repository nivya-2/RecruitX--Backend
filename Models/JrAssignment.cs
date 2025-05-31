namespace RecruitX.Models
{
    public class JrAssignment
    {
        public int Id { get; set; }
        public int JobRequisitionId { get; set; }
        public int AssignedTo { get; set; }
        public int AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public JobRequisition JobRequisition { get; set; } = null!;
        public User AssignedToUser { get; set; } = null!;
        public User AssignedByUser { get; set; } = null!;
    }
}
