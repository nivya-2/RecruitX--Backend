using RecruitX.Models.DTO;
using RecruitX.Models;

namespace RecruitX.Interfaces
{
    public interface IJrAssignmentService
    {
        //Task<JrAssignment> AssignJrAsync(AssignJrDTO dto, string assignedByUsername);
        Task<JrAssignment> AssignJrAsync(AssignJrDTO dto);


    }
}
