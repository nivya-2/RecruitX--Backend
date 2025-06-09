namespace RecruitX.Models.DTO
{
    public record JobRequisitionInfoDto(
    int Id,
    string JobTitle,
    string Department,
    string Location,
    int OpenPositions,
    DateOnly? RequestedDate,
    string HiringManager,
    string RaisedBy,
    string RaisedByRole,
    string Qualification
);


}
