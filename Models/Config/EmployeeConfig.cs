using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class EmployeeConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> entity)
        {
            entity.ToTable("employees");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("Id")
                  .ValueGeneratedOnAdd();


            entity.Property(e => e.FirstName)
                  .HasColumnName("first_name")
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.LastName)
                  .HasColumnName("last_name")
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Email)
                  .HasColumnName("email")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Phone)
                  .HasColumnName("phone");

            entity.Property(e => e.LocationId)
                  .HasColumnName("location_id");

            entity.Property(e => e.Position)
                  .HasColumnName("position")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.DepartmentId)
          .HasColumnName("department_id")
          .IsRequired();

            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_employees_department_id");


            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();



            entity.HasOne(e => e.Location)
                  .WithMany()
                  .HasForeignKey(e => e.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);

        }
    }
}