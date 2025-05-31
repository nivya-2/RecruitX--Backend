using Microsoft.EntityFrameworkCore;

namespace RecruitX.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string? ClientCountry { get; set; } 
    }

}
