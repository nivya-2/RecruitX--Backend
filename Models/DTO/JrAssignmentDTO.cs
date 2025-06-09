namespace RecruitX.Models.DTO
{
    // In a DTOs folder, e.g., /DTOs/JrAssignmentDto.cs
    public class JrAssignmentDto
    {
        public int Id { get; set; }
        public int JobRequisitionId { get; set; }
        public DateTime AssignedAt { get; set; }

        // Instead of just IDs, we use our DTOs to provide useful info
        public UserSummaryDto AssignedTo { get; set; }
        public UserSummaryDto AssignedBy { get; set; }
    }
}
