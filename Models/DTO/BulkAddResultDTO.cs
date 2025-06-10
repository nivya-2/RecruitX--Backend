namespace RecruitX.Models.DTO
{
    public class BulkAddResultDTO
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> FailureMessages { get; set; } = new List<string>();
        public List<int> SuccessfulApplicationIds { get; set; } = new List<int>();

    }
}
