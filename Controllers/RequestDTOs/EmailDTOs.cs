using System.ComponentModel.DataAnnotations;

namespace RecruitX.Controllers.RequestDTOs
{
public class SendScreeningEmailRequest
{
    [Required(ErrorMessage = "Recipient email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string ToEmail { get; set; }

    [Required(ErrorMessage = "Candidate name is required.")]
    public string CandidateName { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    public string JobTitle { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    public string CompanyName { get; set; }

    public string ApplicationDate { get; set; } // Available variable

    [Required(ErrorMessage = "Recruiter name is required.")]
    public string RecruiterName { get; set; }

    public string RecruiterEmail { get; set; } // Available variable

    [Required(ErrorMessage = "Screening date is required.")]
    public string ScreeningDate { get; set; }

    [Required(ErrorMessage = "Screening time is required.")]
    public string ScreeningTime { get; set; }
}

public class SendInterviewInvitationRequest
{
    [Required(ErrorMessage = "Recipient email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string ToEmail { get; set; }

    [Required(ErrorMessage = "Candidate name is required.")]
    public string CandidateName { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    public string JobTitle { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    public string CompanyName { get; set; }

    public string ApplicationDate { get; set; } // Available variable

    [Required(ErrorMessage = "Recruiter name is required.")]
    public string RecruiterName { get; set; }

    [Required(ErrorMessage = "Recruiter email is required.")]
    [EmailAddress(ErrorMessage = "Invalid recruiter email address format.")]
    public string RecruiterEmail { get; set; }

    [Required(ErrorMessage = "Interview date is required.")]
    public string InterviewDate { get; set; }

    [Required(ErrorMessage = "Interview time is required.")]
    public string InterviewTime { get; set; }

    [Required(ErrorMessage = "Interview location is required.")]
    public string InterviewLocation { get; set; }

    [Required(ErrorMessage = "Interviewer names are required.")]
    public string InterviewerNames { get; set; }
}

public class SendJobOfferRequest
{
    [Required(ErrorMessage = "Recipient email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string ToEmail { get; set; }

    [Required(ErrorMessage = "Candidate name is required.")]
    public string CandidateName { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    public string JobTitle { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    public string CompanyName { get; set; }

    public string ApplicationDate { get; set; }
    public string RecruiterName { get; set; }
    public string RecruiterEmail { get; set; }

    [Required(ErrorMessage = "Salary is required.")]
    public string Salary { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    public string StartDate { get; set; }

    [Required(ErrorMessage = "Benefits package is required.")]
    public string BenefitsPackage { get; set; }

    [Required(ErrorMessage = "Manager name is required.")]
    public string ManagerName { get; set; }

    [Required(ErrorMessage = "Response deadline is required.")]
    public string ResponseDeadline { get; set; }

    [Required(ErrorMessage = "HR contact name/department is required.")]
    public string HrContact { get; set; }

    [Required(ErrorMessage = "HR email is required.")]
    [EmailAddress(ErrorMessage = "Invalid HR email address format.")]
    public string HrEmail { get; set; }
}

public class SendRejectionEmailRequest
{
    [Required(ErrorMessage = "Recipient email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string ToEmail { get; set; }

    [Required(ErrorMessage = "Candidate name is required.")]
    public string CandidateName { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    public string JobTitle { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    public string CompanyName { get; set; }

    public string ApplicationDate { get; set; } // Available variable

    [Required(ErrorMessage = "Recruiter name is required.")]
    public string RecruiterName { get; set; }

    public string RecruiterEmail { get; set; } // Available variable
}

public class SendOnboardingEmailRequest
{
    [Required(ErrorMessage = "Recipient email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string ToEmail { get; set; }

    [Required(ErrorMessage = "Employee/Candidate name is required.")]
    public string EmployeeName { get; set; }

    [Required(ErrorMessage = "Role/Job title is required.")]
    public string Role { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    public string CompanyName { get; set; }

    public string ApplicationDate { get; set; }
    public string RecruiterNameForContext { get; set; }
    public string RecruiterEmailForContext { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    public string StartDate { get; set; }

    [Required(ErrorMessage = "Manager name is required.")]
    public string ManagerName { get; set; }

    [Required(ErrorMessage = "Desk location is required.")]
    public string DeskLocation { get; set; }

    [Required(ErrorMessage = "HR contact name/department is required.")]
    public string HrContact { get; set; }

    [Required(ErrorMessage = "HR email is required.")]
    [EmailAddress(ErrorMessage = "Invalid HR email address format.")]
    public string HrEmail { get; set; }
}

}
