namespace RecruitX.Controllers.RequestDTOs
{
    public class SendScreeningEmailRequest
    {
        public int JobRequisitionId { get; set; }
        public int CandidateId { get; set; }
    }
    public class SendInterviewInvitationRequest
    {
        public int InterviewId { get; set; }
    }
}
