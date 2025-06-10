namespace RecruitX.Models.DTO
{
    public class ToScheduleDto
    {
        public int Id { get; set; }
        public string RoleTitle { get; set; }
        public string DeliveryUnit { get; set; }
        public string Location { get; set; }
        public int Experience { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string AssoJr { get; set; }
        public List<string> Actions { get; set; }
    }
}
