using RecruitX.Models;
using System.ComponentModel.DataAnnotations;
using RecruitX.Data;
public class JobSkill
{
    public int Id { get; set; }

    public int JobRequisitionId { get; set; }  
    public int SkillId { get; set; }

    public SkillTypes SkillType { get; set; }

    public JobRequisition JobRequisition { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
    

}


