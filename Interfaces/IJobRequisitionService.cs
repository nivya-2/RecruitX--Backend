using RecruitX.DTOs;
using RecruitX.Models;
using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IJobRequisitionService
    {

        //Task<JobRequisition> CreateJobRequisitionAsync(UploadJrDTO dto, string username);
        Task<JobRequisition> CreateJobRequisitionAsync(UploadJrDTO dto);
        Task<IEnumerable<JobRequisitionDto>> GetAllAsync();
        Task<bool> DeleteJobRequisitionAsync(int id);
        Task<IEnumerable<JobRequisitionSummaryDto>> GetOpenJobSummariesAsync();
        Task<JrAssignmentDto> AssignJrAsync(AssignJrDTO dto, User user);
        Task<IEnumerable<TrackJobRequisitionDTO>> GetAssignedJobRequisitionsAsync(string userEmail);

    }

}
