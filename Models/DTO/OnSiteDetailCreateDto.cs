using System.ComponentModel.DataAnnotations;
using RecruitX.Data;

namespace RecruitX.Models.DTO
{
    public class OnSiteDetailCreateDto
    {
        public ContractType? ContractType { get; set; } // Maps to OnSiteDetail.ContractType

        [Required]
        [StringLength(100)]
        public string Rate { get; set; } // Maps to OnSiteDetail.Rate

        [Required]
        [StringLength(100)]
        public string WorkLocation { get; set; } // Maps to OnSiteDetail.WorkLocation

        [Required]
        [StringLength(100)]
        public string PreferredVisaStatus { get; set; } // Maps to OnSiteDetail.PreferredVisaStatus

        [Required]
        [StringLength(100)]
        public string ContractDuration { get; set; } // Maps to OnSiteDetail.ContractDuration

        [Required]
        [StringLength(50)]
        public string PreferredTimeZone { get; set; } // Maps to OnSiteDetail.PreferredTimeZone

        [Required]
        public string ClientBackground { get; set; } // Maps to OnSiteDetail.ClientBackground

        [Required]
        [StringLength(100)]
        public string ClientLocation { get; set; } // Maps to OnSiteDetail.ClientLocation

        [Required]
        [StringLength(100)]
        public string ReportingTo { get; set; } // Maps to OnSiteDetail.ReportingTo

        public string? InterviewProcess { get; set; } // Maps to OnSiteDetail.InterviewProcess

        [Required]
        public DateOnly IdealStartDate { get; set; } // Maps to OnSiteDetail.IdealStartDate

        public bool? IsH1TransferAccepted { get; set; } // Maps to OnSiteDetail.IsH1TransferAccepted

        public bool? IsTravelRequired { get; set; } // Maps to OnSiteDetail.IsTravelRequired
    }
}
