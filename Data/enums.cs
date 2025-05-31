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
    public enum ApplicationStatus
    {
        Applied,
        TechnicalInterview,
        ManagementInterview,
        DocumentationVerified,
        SalaryApproved,
        OfferLetterAccepted,
        Joined
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



}
