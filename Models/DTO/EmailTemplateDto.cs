using RecruitX.Data;

namespace RecruitX.Models.DTO
{
    public class EmailTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public UserType UserType { get; set; }
        public List<string>? Variables { get; set; } = new();
    }
}
