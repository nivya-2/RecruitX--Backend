using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class ApplicationSkillConfig : IEntityTypeConfiguration<ApplicationSkill>
    {
        public void Configure(EntityTypeBuilder<ApplicationSkill> entity)
        {
            entity.ToTable("application_skills");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("Id")
                  .UseIdentityAlwaysColumn();

            entity.Property(e => e.ApplicationId)
                  .HasColumnName("application_id")
                  .IsRequired();

            entity.Property(e => e.SkillId)
                  .HasColumnName("skill_id")
                  .IsRequired();

            entity.HasOne(e => e.Application)
                  .WithMany(a => a.ApplicationSkills)
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_application_skills_application_id");

            entity.HasOne(e => e.Skill)
                  .WithMany()
                  .HasForeignKey(e => e.SkillId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_application_skills_skill_id");

            entity.HasIndex(e => new { e.ApplicationId, e.SkillId })
                  .IsUnique()
                  .HasDatabaseName("uq_application_skills_application_skill");
        }
    }
}