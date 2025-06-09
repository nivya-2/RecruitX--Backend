using Microsoft.EntityFrameworkCore;
using RecruitX.Models.DTO;
using RecruitX.Models;
using RecruitX.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;

namespace RecruitX.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDetailsDTO>> GetEmployeeDetailsAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Location)
                .Include(e => e.User)
                    .ThenInclude(u => u.Role)
                    .IgnoreQueryFilters()
                .Where(e => e.User != null) // ✅ Keep only employees that have a User
                .Select(e => new UserDetailsDTO
                {
                    UserId = e.User!.Id, // Safe because of the above filter
                    EmployeeId = $"EMP{e.Id:D6}",
                    Name = $"{e.FirstName} {e.LastName}",
                    JobTitle = e.Position,
                    RoleTitle = e.User.Role != null ? e.User.Role.RoleName : "N/A",
                    Location = e.Location != null ? e.Location.LocationName : "N/A",
                    DeliveryUnit = e.Department != null ? e.Department.Name : "N/A",
                    Email = e.Email,
                    Status = e.User.IsActive ? "Active" : "Inactive" // ✅ Now reflects real status
                })
                .ToListAsync();
        }


        public async Task<bool> SetUserInactiveAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (!user.IsActive)
                throw new InvalidOperationException("User is already inactive.");

            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SetUserActiveAsync(int userId)
        {
            var user = await _context.Users
                                     .IgnoreQueryFilters()
                                     .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            if (user.IsActive)
                throw new InvalidOperationException("User is already active.");

            user.IsActive = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SetRecruiterHeadAsync(int userId)
        {
            var recruiterHeadRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Recruiter Head");
            var recruiterRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Recruiter");

            if (recruiterHeadRole == null || recruiterRole == null)
                throw new InvalidOperationException("Roles not properly configured.");

            var newHead = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (newHead == null)
                return false;

            if (newHead.RoleId == recruiterHeadRole.Id)
                return false; // Already recruiter head

            // Demote current recruiter head (if exists)
            var currentHead = await _context.Users
                .FirstOrDefaultAsync(u => u.RoleId == recruiterHeadRole.Id);

            if (currentHead != null)
            {
                currentHead.RoleId = recruiterRole.Id;
                _context.Users.Update(currentHead);
            }

            // Promote the new head
            newHead.RoleId = recruiterHeadRole.Id;
            _context.Users.Update(newHead);

            await _context.SaveChangesAsync();
            return true;
        }



    }
}
