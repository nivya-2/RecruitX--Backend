namespace RecruitX.Models.DTO
{
    // In a DTOs folder, e.g., /DTOs/UserSummaryDto.cs
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } // Or Username, Email, etc.
                                             // Notice: NO Employee or other complex navigation properties.
    }
}
