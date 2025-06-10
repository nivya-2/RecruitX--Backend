using System.Text.Json.Serialization;

namespace RecruitX.Models.DTO
{
    public class TeamMemberDTO
    {
        public int UserId { get; set; }
        public string MemberName { get; set; }
        public string JobTitle { get; set; }
        public int JrAssigned { get; set; }
        public string? ReportingLead { get; set; }
        public List<string> Actions { get; set; }
    }
}
