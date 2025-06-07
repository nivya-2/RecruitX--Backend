using RecruitX.Models;
using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface ITrackJdService
    {
        Task<IEnumerable<TrackJdDTO>> GetJobDescriptionsForUserAsync(string userEmail);
        Task<IEnumerable<PendingJdDTO>> GetPendingJdsForUserAsync(string userEmail);
        Task<JobDescriptionDTO> GetJobDescriptionDetailsAsync(int jobRequisitionId);

        Task<IEnumerable<JdApplicantsDTO>> GetApplicantsForJdAsync(int jobDescriptionId);

        Task<JobDescriptionDTO> GenerateJobDescriptionFromRequisitionAsync(int jobRequisitionId);
        Task<bool> SaveDraftJobDescriptionAsync(JobDescriptionDTO dto, string userEmail);
        Task<CandidateDetailsDTO?> GetCandidateDetailsByApplicationIdAsync(int applicationId);

        Task<bool> SubmitJobDescriptionAsync(JobDescriptionDTO dto, string userEmail);



    }
}
