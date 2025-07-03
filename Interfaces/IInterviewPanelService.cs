using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IInterviewPanelService
    {
        Task<IEnumerable<PanelMemberNameAndEmailDto>> GetPanelMembersAsync();

        Task<IEnumerable<InterviewMeetingDetailsDto>> GetPanelMemberMeetingsAsync(string email, DateTime date);
        Task<ScheduleInterviewResponseDto> ScheduleInterviewAsync(ScheduleInterviewRequestDto request, string organizerEmail);
    }
}
