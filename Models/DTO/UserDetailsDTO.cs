namespace RecruitX.Models.DTO
{
    public class UserDetailsDTO
    {
        public int UserId { get; set; }                    // ✅ Add this
        public string EmployeeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string RoleTitle { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string DeliveryUnit { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "Inactive";
    }
}
