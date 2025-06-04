using System.ComponentModel.DataAnnotations;
using RecruitX.Data;

namespace RecruitX.Models.DTO
{
    public class JobSkillInputDto
    {
        [Required]
        [StringLength(100)]
        public string SkillName { get; set; } // The name of the skill

        [Required]
        public SkillTypes SkillType { get; set; }
    }
}
