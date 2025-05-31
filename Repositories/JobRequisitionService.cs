using Microsoft.EntityFrameworkCore;
using RecruitX.Models;

public sealed class JobRequisitionService(AppDbContext dbContext) : IJobRequisitionService
{
    public async Task<List<TrackJrDto>> GetTrackJrAsync()
    {
        var data = await dbContext.JobRequisitions
            .Include(jr => jr.Location)
            .Include(jr => jr.HiringManagerEmployee)
            .Include(jr => jr.CreatedByEmployee)
            .Select(jr => new
            {
                jr.Id,
                jr.Role,
                jr.NumPositions,
                jr.LocationId,
                jr.Location.LocationName,
                HiringManager = jr.HiringManagerEmployee,
                CreatedBy = jr.CreatedByEmployee,
                jr.CreatedAt,
                jr.IdealStartDate,
                FillCount = dbContext.JobDescriptions
                    .Where(jd => jd.JobRequisitionId == jr.Id)
                    .Sum(jd => (int?)jd.FilledPositions) ?? 0
            })
            .ToListAsync();

        return data.Select(jr => new TrackJrDto(
            JobId: jr.Id.ToString(),
            JobTitle: jr.Role,
            //update the Deparment Foreign key mapping in DTO and this
            Du: "N/A",
            Location: jr.LocationName,
            Status: new StatusDto(
                Filled: jr.FillCount,
                Vacancies: (jr.NumPositions ?? 0) - jr.FillCount
            ),
            HiringManager: $"{jr.HiringManager.FirstName} {jr.HiringManager.LastName}",
            AssignedTo: $"{jr.CreatedBy.FirstName} {jr.CreatedBy.LastName}",
            AssignedOn: jr.CreatedAt.ToString("yyyy-MM-dd"),
            CloseBy: jr.IdealStartDate?.ToString("yyyy-MM-dd") ?? "N/A"
        )).ToList();
    }


}
