namespace RecruitX.Controllers.RequestDTOs
{
    /// <summary>
    /// DTO for sending emails related to a specific application.
    /// The Application entity provides all the necessary context.
    /// </summary>
    public class ApplicationEmailRequest
    {
        public int ApplicationId { get; set; }
    }

    /// <summary>
    /// DTO for sending an interview invitation.
    /// The InterviewId is the most specific identifier for this action.
    /// </summary>
    public class SendInterviewInvitationRequest
    {
        public int InterviewId { get; set; }
        public string? MeetingLink { get; set; }

    }
}