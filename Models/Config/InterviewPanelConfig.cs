using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class InterviewPanelConfig : IEntityTypeConfiguration<InterviewPanel>
    {
        public void Configure(EntityTypeBuilder<InterviewPanel> entity)
        {
            entity.ToTable("interview_panel");

            // Primary key
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("Id");

            entity.Property(e => e.InterviewId).HasColumnName("interview_id").IsRequired();
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id").IsRequired();

            // Unique constraint on composite key
            entity.HasIndex(e => new { e.InterviewId, e.EmployeeId }).IsUnique();

            // Relationships
            entity.HasOne(ip => ip.Interview)
                  .WithMany(i => i.InterviewPanels)
                  .HasForeignKey(ip => ip.InterviewId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ip => ip.Employee)
                  .WithMany()
                  .HasForeignKey(ip => ip.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}