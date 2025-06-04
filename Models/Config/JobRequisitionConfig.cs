using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Data;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class JobRequisitionConfig : IEntityTypeConfiguration<JobRequisition>
    {
        public void Configure(EntityTypeBuilder<JobRequisition> entity)
        {
            entity.ToTable("job_requisitions");

            entity.HasKey(j => j.Id);
            entity.Property(j => j.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(JobStatus.Open)
            .IsRequired();
            entity.Property(j => j.DepartmentId)
            .HasColumnName("department_id") 
            .IsRequired();

            entity.Property(j => j.RequestedBy);
            entity.Property(j => j.HiringManager);
            entity.Property(j => j.NumPositions);
            entity.Property(j => j.Role).IsRequired().HasMaxLength(100);
            entity.Property(j => j.Qualification).HasMaxLength(100);

            // ✅ Separated experience columns (int) instead of decimal
            entity.Property(j => j.TotalExperienceYears);
            entity.Property(j => j.RelevantExperienceYears);

            entity.Property(j => j.LocationId);
            entity.Property(j => j.JobPurpose);
            entity.Property(j => j.JobSpecification);
            entity.Property(j => j.ProjectName).HasMaxLength(100);
            entity.Property(j => j.ProjectRole).HasMaxLength(100);
            entity.Property(j => j.HasOnsiteOpportunity).HasDefaultValue(false);
            entity.Property(j => j.IsBillable);
            entity.Property(j => j.HasClientInterview);
            entity.Property(j => j.ClientId);
            entity.Property(j => j.ExpectedSalaryMinimum);
            entity.Property(j => j.ExpectedSalaryMaximum);
            entity.Property(j => j.IsClosed);
            entity.Property(j => j.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(j => j.CreatedBy);


            entity.HasOne(j => j.RequestedByEmployee)
                .WithMany()
                .HasForeignKey(j => j.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(j => j.HiringManagerEmployee)
                .WithMany()
                .HasForeignKey(j => j.HiringManager)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(j => j.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(j => j.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(j => j.Client)
                .WithMany()
                .HasForeignKey(j => j.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(j => j.Location)
                .WithMany()
                .HasForeignKey(j => j.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(j => j.RequestedDate)
    .HasColumnType("date"); // Ensure EF maps this as SQL DATE

            entity.Property(j => j.ExpectedOnboardingDate)
                .HasColumnType("date");

            entity.Property(j => j.IdealStartDate)
                .HasColumnType("date");

            entity.Property(j => j.WorkShift)
                .HasConversion<string>() // Store enum as string
                .HasMaxLength(20)
                .IsRequired();

            //entity.HasOne(j => j.Department) 
            //      .WithMany(d => d.JobRequisitions) 
            //      .HasForeignKey(j => j.DepartmentId)
            //      .OnDelete(DeleteBehavior.Restrict) 
            //      .IsRequired();
        }
    }
}