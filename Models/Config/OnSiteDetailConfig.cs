using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class OnSiteDetailConfig : IEntityTypeConfiguration<OnSiteDetail>
    {
        public void Configure(EntityTypeBuilder<OnSiteDetail> entity)
        {
            entity.ToTable("on_site_details");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.Property(e => e.JrId).HasColumnName("jr_id");
            entity.HasIndex(e => e.JrId).IsUnique(); // Ensures jr_id is unique

            entity.Property(e => e.Rate).HasColumnName("rate").IsRequired();
            entity.Property(e => e.IdealStartDate).HasColumnName("ideal_start_date").IsRequired();
            entity.Property(e => e.ContractType).HasColumnName("contract_type");
            entity.Property(e => e.ContractDuration).HasColumnName("contract_duration").IsRequired();
            entity.Property(e => e.ReportingTo).HasColumnName("reporting_to").IsRequired();
            entity.Property(e => e.PreferredTimeZone).HasColumnName("preferred_time_zone").IsRequired();
            entity.Property(e => e.PreferredVisaStatus).HasColumnName("preferred_visa_status").IsRequired();
            entity.Property(e => e.IsH1TransferAccepted).HasColumnName("h1_transfer_accepted");
            entity.Property(e => e.InterviewProcess).HasColumnName("interview_process");
            entity.Property(e => e.IsTravelRequired).HasColumnName("travel_required");
            entity.Property(e => e.ClientBackground).HasColumnName("client_background").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.JobRequisition)
                  .WithOne()
                  .HasForeignKey<OnSiteDetail>(e => e.JrId)
                  .HasConstraintName("FK_OnsiteJobDetail_JobRequisition");
        }
    }
}