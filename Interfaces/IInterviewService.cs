using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IInterviewService
    {
        Task<IEnumerable<InterviewDTO>> GetAllInterviewsAsync();
        Task<IEnumerable<ToScheduleDto>> GetToScheduleInterviewsAsync();
        Task<IEnumerable<ToShortlistDto>> GetInterviewsToShortlistAsync();

    }
}
