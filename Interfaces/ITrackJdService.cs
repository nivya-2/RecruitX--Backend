using RecruitX.Models;
using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface ITrackJdService
    {
        Task<IEnumerable<TrackJdDTO>> GetJobDescriptionsForUserAsync(string userEmail);
        Task<IEnumerable<PendingJdDTO>> GetPendingJdsForUserAsync(string userEmail);

    }
}
