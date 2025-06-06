using RecruitX.Models.DTO;
using RecruitX.Models;

namespace RecruitX.Interfaces
{
    public interface IJrAssignmentService
    {
        Task<JrAssignment> AssignJobRequisitionAsync(AssignJrDTO assignDto);

    }
}
