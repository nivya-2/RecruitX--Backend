using RecruitX.Models.DTO;
using RecruitX.Models;

namespace RecruitX.Interfaces
{
    public interface IAssignedJrService
    {
        Task<List<AssignedJrDTO>> GetAssignedJobRequisitionsByUserNameAsync(int userId);
    }
}
