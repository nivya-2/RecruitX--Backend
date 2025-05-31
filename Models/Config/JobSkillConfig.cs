using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Data;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class JobSkillConfig : IEntityTypeConfiguration<JobSkill>
    {
        public void Configure(EntityTypeBuilder<JobSkill> entity)
        {
            entity.ToTable("job_skills");

            entity.HasKey(e => e.Id)
                  .HasName("Id");

            entity.Property(e => e.Id)
                  .HasColumnName("Id");

            entity.Property(e => e.JobRequisitionId)
                  .HasColumnName("jr_id")
                  .IsRequired();

            entity.Property(e => e.SkillId)
                  .HasColumnName("skill_id")
                  .IsRequired();

            entity.Property(e => e.SkillType)
                  .HasColumnName("skill_type")
                  .HasConversion<string>() // store enum as string
                  .HasMaxLength(20)
                  .HasDefaultValue(SkillTypes.Mandatory)
                  .IsRequired();

            entity.HasOne(e => e.JobRequisition)
                  .WithMany(jr => jr.JobSkills)
                  .HasForeignKey(e => e.JobRequisitionId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_jobskills_jr_id");

            entity.HasOne(e => e.Skill)
                  .WithMany() // no navigation property on Skill
                  .HasForeignKey(e => e.SkillId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_jobskills_skill_id");
        }
    }
}