namespace RecruitX.Models.DTO
{
    public class JobRequisitionSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
        public int? OpenPositions { get; set; }
        public DateOnly? PostedDate { get; set; }
        public string? Location { get; set; }
    }

}
