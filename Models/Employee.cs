using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecruitX.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long? Phone { get; set; }
        public string Position { get; set; } = string.Empty;
        public int DepartmentId { get; set; }         
        public Department Department { get; set; } = null!; 

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User? User { get; set; }
    }

}
