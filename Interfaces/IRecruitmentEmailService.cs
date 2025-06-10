public interface IRecruitmentEmailService
{
    Task SendScreeningEmailAsync(int applicationId);
    Task SendJobOfferEmailAsync(int applicationId);
    Task SendRejectionEmailAsync(int applicationId);
    Task SendInterviewInvitationAsync(int interviewId, string? meetingLink);
}
