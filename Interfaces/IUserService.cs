using RecruitX.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecruitX.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDetailsDTO>> GetEmployeeDetailsAsync();
        Task<bool> SetUserInactiveAsync(int userId);
        Task<bool> SetUserActiveAsync(int userId);
        Task<bool> SetRecruiterHeadAsync(int userId);


    }
}
