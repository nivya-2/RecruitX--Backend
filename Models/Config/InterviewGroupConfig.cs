using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class InterviewGroupConfig : IEntityTypeConfiguration<InterviewerGroup>
    {
        public void Configure(EntityTypeBuilder<InterviewerGroup> entity)
        {
            entity.ToTable("interviewer_group");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.DepartmentId).HasColumnName("delivery_unit");

            entity.HasOne(e => e.Department)
                  .WithMany()
                  .HasForeignKey(e => e.DepartmentId);
        }
    }
}