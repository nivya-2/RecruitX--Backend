using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RecruitX.Data;

namespace RecruitX.Models
{
    public class JobRequisition
    {
        [Key]
        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public DateOnly? RequestedDate { get; set; }

        public int? RequestedBy { get; set; }

        public int? HiringManager { get; set; }

        public int? NumPositions { get; set; }

        public JobStatus JrStatus { get; set; }

        public WorkShiftTypes WorkShift { get; set; } // Changed from string to enum

        public DateOnly? ExpectedOnboardingDate { get; set; }
        public string Role { get; set; }

        public string Qualification { get; set; }

        public string JobDuties { get; set; }

        public int? TotalExperienceYears { get; set; }
        // Removed TotalExperienceMonths

        public int? RelevantExperienceYears { get; set; }
        // Removed RelevantExperienceMonths

        public int? LocationId { get; set; }

        public string JobPurpose { get; set; }

        public string JobSpecification { get; set; }

        public string ProjectName { get; set; }

        public string ProjectRole { get; set; }

        public bool HasOnsiteOpportunity { get; set; }

        public bool? IsBillable { get; set; }

        public bool? HasClientInterview { get; set; }

        public int? ClientId { get; set; }

        public int? ExpectedSalaryMinimum { get; set; }

        public int? ExpectedSalaryMaximum { get; set; }

        public DateOnly? IdealStartDate { get; set; }

        public bool IsClosed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        // Navigation
        public Employee RequestedByEmployee { get; set; }
        public Employee HiringManagerEmployee { get; set; }
        public Employee CreatedByEmployee { get; set; }
        public Client Client { get; set; }
        public Location Location { get; set; }
        public Department Department { get; set; }
        public Status JDstatus { get; set; }
        public DateTime? DeletedAt { get; set; } // ✅ Soft delete

        [JsonIgnore]
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}
