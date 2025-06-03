using RecruitX.Data;
using System.ComponentModel.DataAnnotations;

namespace RecruitX.Models.DTO
{
    public class UploadJrDTO
    {
   
        [Required]
        [StringLength(100)]
        public string Role { get; set; }

        public DateTime? RequestedDate { get; set; }

        public string? RequestedByName { get; set; } 

        [Required]
        public string DepartmentName { get; set; }

        public string? HiringManagerName { get; set; } 

        public List<JobSkillInputDto> Skills { get; set; } = new List<JobSkillInputDto>();

        [Required]
        [StringLength(100)]
        public string Qualification { get; set; }

        public short? TotalExperienceYears { get; set; }
        [Range(0, 11, ErrorMessage = "Months must be between 0 and 11.")]
        public short? TotalExperienceMonths { get; set; }

        public short? RelevantExperienceYears { get; set; }
        [Range(0, 11, ErrorMessage = "Months must be between 0 and 11.")]
        public short? RelevantExperienceMonths { get; set; }

        public string? LocationName { get; set; } // To create a new Location
        public string? LocationCountry { get; set; } // To create a new Location

        public DateTime? ExpectedOnboardingDate { get; set; }

        public int? NumPositions { get; set; }

        [Required]
        [StringLength(50)]
        public string WorkShift { get; set; }

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

        public string? ClientName { get; set; } // To create a new Client
        public string? ClientCountry { get; set; } // To create a new Client

        // Expected Salary Range
        public int? ExpectedSalaryMinimum { get; set; }
        public int? ExpectedSalaryMaximum { get; set; }

        [Required]
        public string JobPurpose { get; set; }

        [Required]
        public string JobDuties { get; set; }

        [Required]
        public string JobSpecification { get; set; }

        [Required]
        [StringLength(50)]
        public string WorkModel { get; set; } // Maps to JobRequisition.WorkModel

        public DateTime? IdealStartDate { get; set; } // Maps to JobRequisition.IdealStartDate


        public OnSiteDetailCreateDto? OnSiteDetails { get; set; }


        public string? CreatedByEmployeeName { get; set; }

        public JobStatus Status {  get; set; }
    }


}




