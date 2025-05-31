public record TrackJrDto(
    string JobId,
    string JobTitle,
    string Du,
    string Location,
    StatusDto Status,
    string HiringManager,
    string AssignedTo,
    string AssignedOn,
    string CloseBy
);

public sealed record StatusDto(int Filled, int Vacancies);
