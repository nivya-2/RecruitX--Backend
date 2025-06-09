namespace RecruitX.Interfaces
{
    public class IInterviewService
    {
        Task<IEnumerable<InterviewDTO>> GetAllInterviewsAsync();
        Task<IEnumerable<ToScheduleDto>> GetToScheduleInterviewsAsync();
        Task<IEnumerable<ToShortlistDto>> GetInterviewsToShortlistAsync();

    }
}
