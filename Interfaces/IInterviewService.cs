using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IInterviewService
    {
        Task<IEnumerable<InterviewDTO>> GetAllInterviewsAsync();
        Task<IEnumerable<ToScheduleDto>> GetToScheduleInterviewsAsync();
        Task<IEnumerable<ToShortlistDto>> GetInterviewsToShortlistAsync();
        Task<List<CandidateDTO>> GetCandidatesByJobDescriptionIdAsync(int jdId);
        Task<bool> ShortlistCandidateAsync(int jobRequisitionId, int candidateId);
        Task<bool> RejectCandidateAsync(int jobRequisitionId, int candidateId);
        Task<bool> IncrementInterviewCountAsync(int jobRequisitionId, int candidateId);

    }
}
