namespace RecruitX.Models
{
    public class JobDescription
    {
        public int Id { get; set; }
        public int JobRequisitionId { get; set; }
        public string? JobDesc { get; set; }
        public int? FilledPositions { get; set; }
        public string? Updates { get; set; }  // Assuming JSON stored as string
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }

        // Navigation properties (optional)
        public JobRequisition? JobRequisition { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}
