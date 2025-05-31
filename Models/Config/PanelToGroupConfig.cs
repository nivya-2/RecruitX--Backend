using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class PanelToGroupConfig : IEntityTypeConfiguration<PanelToGroup>
    {
        public void Configure(EntityTypeBuilder<PanelToGroup> entity)
        {
            entity.ToTable("panel_to_group");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.GroupId).HasColumnName("Id").IsRequired();
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id").IsRequired();

            entity.HasIndex(e => new { e.GroupId, e.EmployeeId })
                  .IsUnique();

            entity.HasOne(e => e.Group)
                  .WithMany(g => g.PanelMembers)
                  .HasForeignKey(e => e.GroupId);

            entity.HasOne(e => e.Employee)
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId);
        }
    }
}