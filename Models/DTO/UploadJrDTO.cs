using RecruitX.Data;
using System.ComponentModel.DataAnnotations;

namespace RecruitX.Models.DTO
{
    public class UploadJrDTO
    {
        [Required]
        [StringLength(100)]
        public string Role { get; set; }

        public DateOnly? RequestedDate { get; set; }

        public string? RequestedByName { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        public string? HiringManagerName { get; set; }

        public List<JobSkillInputDto> Skills { get; set; } = new();

        [Required]
        [StringLength(100)]
        public string Qualification { get; set; }

        public short? TotalExperienceYears { get; set; }
        // Removed TotalExperienceMonths

        public short? RelevantExperienceYears { get; set; }
        // Removed RelevantExperienceMonths

        public string? LocationName { get; set; }
        public string? LocationCountry { get; set; }

        public DateOnly? ExpectedOnboardingDate { get; set; }

        public int? NumPositions { get; set; }

        [Required]
        public WorkShiftTypes WorkShift { get; set; }

        [Required]
        public bool HasOnsiteOpportunity { get; set; }

        public bool? IsBillable { get; set; }

        public bool? HasClientInterview { get; set; }

        [Required]
        [StringLength(100)]
        public string ProjectName { get; set; }

        [Required]
        [StringLength(100)]
        public string ProjectRole { get; set; }

        public string? ClientName { get; set; }
        public string? ClientCountry { get; set; }

        public int? ExpectedSalaryMinimum { get; set; }
        public int? ExpectedSalaryMaximum { get; set; }

        [Required]
        public string JobPurpose { get; set; }

        [Required]
        public string JobDuties { get; set; }

        [Required]
        public string JobSpecification { get; set; }

        public DateOnly? IdealStartDate { get; set; }

        public OnSiteDetailCreateDto? OnSiteDetails { get; set; }

        public string? CreatedByEmployeeName { get; set; }

        public JobStatus Status { get; set; }
    }
}
