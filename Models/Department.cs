namespace RecruitX.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<JobRequisition> JobRequisitions{ get; set; } = new List<JobRequisition>();

    }

}
