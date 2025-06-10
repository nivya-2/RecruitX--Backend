using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamMemberDTO>> GetTeamMembersAsync();
        Task<IEnumerable<TeamMemberDTO>> GetRecruitersForLeadAsync(string leadUserEmail);
    }
}

