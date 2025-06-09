// File Path: Interfaces/IInterviewPanelService.cs

using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IInterviewPanelService
    {
        Task<object> TestGraphConnectionAsync();
        Task<IEnumerable<PanelMemberNameAndEmailDto>> GetPanelMembersAsync();
        //Task<IEnumerable<PanelMemberDto>> GetPanelMembersWithDetailsAsync();
        Task<IEnumerable<InterviewMeetingDetailsDto>> GetPanelMemberMeetingsAsync(string email, DateTime date);
        Task<ScheduleInterviewResponseDto> ScheduleInterviewAsync(ScheduleInterviewRequestDto request, string organizerEmail);
    }
}