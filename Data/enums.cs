namespace RecruitX.Data
{
    public enum SkillTypes
    {
        GoodToHave,
        Mandatory,
        Primary
    }
    public enum JobStatus
    {
        Open,
        Closed,
        OnHold
    }

    public enum Status
    {
        GenerateJD,
        Draft,
        Generated
    }
    public enum ApplicationStatus
    {
        Applied,
        TechnicalInterview,
        ManagementInterview,
        DocumentationVerified,
        SalaryApproved,
        OfferLetterAccepted,
        Joined,

        Rejected = 99

    }
    public enum InterviewStatus
    {
        Scheduled,
        PendingShortlist,
        Completed
    }
    public enum UserType
    {
        Candidate,
        Recruiter,
        InterviewPanel
    }
    public enum WorkShiftTypes
    {
        General,
        UK,
        US
    }

    public enum ContractType
    {
        Contract,
        Permanent
    }


}
