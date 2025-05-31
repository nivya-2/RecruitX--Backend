using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> entity)
        {
            entity.ToTable("departments");

            entity.HasKey(d => d.Id);

            entity.Property(d => d.Id)
                  .HasColumnName("Id");

            entity.Property(d => d.Name)
                  .HasColumnName("name")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.HasMany(d => d.Employees)
                  .WithOne(e => e.Department)
                  .HasForeignKey(e => e.DepartmentId);
        }
    }
    }
