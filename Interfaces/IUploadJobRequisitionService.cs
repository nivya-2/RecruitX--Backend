using RecruitX.Models;
using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IUploadJobRequisitionService
    {

        //Task<JobRequisition> CreateJobRequisitionAsync(UploadJrDTO dto, string username);
        Task<JobRequisition> CreateJobRequisitionAsync(UploadJrDTO dto);


    }

}
