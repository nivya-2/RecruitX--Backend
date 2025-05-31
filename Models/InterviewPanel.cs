namespace RecruitX.Models
{
    public class InterviewPanel
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public Interview Interview { get; set; } = default!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;
    }

}
