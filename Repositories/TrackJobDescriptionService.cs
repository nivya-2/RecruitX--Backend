using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitX.Data;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class TrackJobDescriptionService : ITrackJdService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TrackJobDescriptionService> _logger;

        public TrackJobDescriptionService(AppDbContext context, ILogger<TrackJobDescriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TrackJdDTO>> GetJobDescriptionsForUserAsync(string userEmail)
        {
            try
            {
                _logger.LogInformation("Fetching job descriptions for user email: {UserEmail}", userEmail);

                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("User with email {UserEmail} not found.", userEmail);
                    return Enumerable.Empty<TrackJdDTO>();
                }

                var userId = user.Id;

                // Query JrAssignments where AssignedTo == current user and JDstatus is NOT Generated (for example)
                var jobDescriptions = await _context.JrAssignments
                    .Where(assign => assign.AssignedTo == userId && assign.JobRequisition.JDstatus == Status.Generated)
                    .Join(
                        _context.JobDescriptions,
                        assign => assign.JobRequisitionId,
                        jd => jd.JobRequisitionId,
                        (assign, jd) => new { assign, jd }
                    )
                    .Select(result => new TrackJdDTO
                    {
                        JobRequisitionId = result.jd.JobRequisitionId,
                        RoleTitle = result.assign.JobRequisition.Role,
                        BusinessUnit = result.assign.JobRequisition.Department != null
                            ? result.assign.JobRequisition.Department.Name
                            : "N/A",
                        CreatedDate = DateOnly.FromDateTime(result.jd.CreatedAt),
                        JobStatus = result.assign.JobRequisition.JrStatus
                    })
                    .ToListAsync();

                return jobDescriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job descriptions for user email {UserEmail}", userEmail);
                throw;
            }
        }


        public async Task<IEnumerable<PendingJdDTO>> GetPendingJdsForUserAsync(string userEmail)
        {
            try
            {
                _logger.LogInformation("Fetching pending JDs for user email: {UserEmail}", userEmail);

                // Step 1: Get user
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("User with email {UserEmail} not found.", userEmail);
                    return Enumerable.Empty<PendingJdDTO>();
                }

                var userId = user.Id;

                // Step 2: Get assigned JR IDs
                var userAssignments = await _context.JrAssignments
                    .Where(a => a.AssignedTo == userId)
                    .Select(a => a.JobRequisitionId)
                    .Distinct()
                    .ToListAsync();

                if (!userAssignments.Any())
                {
                    _logger.LogInformation("No JR assignments found for user {UserId}", userId);
                    return Enumerable.Empty<PendingJdDTO>();
                }

                // Step 3: Get JRs that already have JDs


                // Step 4: Fetch JRs assigned to user that DO NOT have a JD
                var pendingJds = await _context.JobRequisitions
                                .Where(jr => userAssignments.Contains(jr.Id) &&
                                 jr.JDstatus != Status.Generated) 
                                 .Include(jr => jr.Department)
                                 .Include(jr => jr.HiringManagerEmployee)
                                 .Include(jr => jr.Location)
                                 .Select(jr => new PendingJdDTO
                                 {
        JobRequisitionId = jr.Id,
        RoleTitle = jr.Role,
        BusinessUnit = jr.Department != null ? jr.Department.Name : "N/A",
        location = jr.Location != null ? jr.Location.LocationName : "N/A",
        openPositions = jr.NumPositions ?? 0,
        HiringManager = jr.HiringManagerEmployee != null ?
            $"{jr.HiringManagerEmployee.FirstName} {jr.HiringManagerEmployee.LastName}" : "N/A",
        CreatedDate = DateOnly.FromDateTime(jr.CreatedAt),
        //JobStatus = jr.JDstatus.G // Or jr.JdStatus if needed
    })
    .ToListAsync();

                //_logger.LogInformation("Found {Count} pending JDs for user {UserId}", pendingJds.Count, userId);
                return pendingJds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending JDs for user {UserEmail}", userEmail);
                throw;
            }
        }

    }
}
