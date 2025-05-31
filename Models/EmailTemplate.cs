using RecruitX.Data;

namespace RecruitX.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public UserType UserType { get; set; } 
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Creator { get; set; }
    }
}
