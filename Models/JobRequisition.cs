using System.ComponentModel.DataAnnotations;
using RecruitX.Data;

namespace RecruitX.Models

{

    public class JobRequisition

    {
        [Key]
        public int Id { get; set; }

        public string BusinessUnit { get; set; }

        public DateTime? RequestedDate { get; set; }

        public int? RequestedBy { get; set; }

        public int? HiringManager { get; set; }

        public int? NumPositions { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Open;

        public string WorkShift { get; set; }

        public DateTime? ExpectedOnboardingDate { get; set; }

        public string WorkModel { get; set; }

        public string Role { get; set; }

        public string Qualification { get; set; }
        public string JobDuties { get; set; }


        public int? TotalExperienceYears { get; set; }
        public int? TotalExperienceMonths { get; set; }

        public int? RelevantExperienceYears { get; set; }
        public int? RelevantExperienceMonths { get; set; }


        public int? LocationId { get; set; }

        public string JobPurpose { get; set; }

        public string JobSpecification { get; set; }

        public string ProjectName { get; set; }

        public string ProjectRole { get; set; }

        public bool HasOnsiteOpportunity { get; set; }

        public bool? IsBillable { get; set; }

        public bool? HasClientInterview { get; set; }

        public int? ClientId { get; set; }

        public string ExpectedSalaryRange { get; set; }

        public DateTime? IdealStartDate { get; set; }

        public Boolean IsClosed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        // Navigation properties

        public Employee RequestedByEmployee { get; set; }

        public Employee HiringManagerEmployee { get; set; }

        public Employee CreatedByEmployee { get; set; }

        public Client Client { get; set; }

        public Location Location { get; set; }
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();



    }

}

