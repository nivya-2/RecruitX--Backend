using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class JrForeignKeysDepartmentMinMaxSaaryEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)

        {

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "job_requisitions",
                type:"int",
                nullable: false,
                defaultValue:1
               );

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_department_id",
                table: "job_requisitions",
                 column: "department_id");

            migrationBuilder.AddForeignKey(
                name: "FK_job_requisitions_departments_department_id",
                table: "job_requisitions",
                column: "department_id",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_requisitions_departments_department_id",
                table: "job_requisitions");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "job_requisitions",
                newName: "business_unit_id");

            migrationBuilder.RenameIndex(
                name: "IX_job_requisitions_department_id",
                table: "job_requisitions",
                newName: "IX_job_requisitions_business_unit_id");

            migrationBuilder.AddForeignKey(
                name: "FK_job_requisitions_departments_business_unit_id",
                table: "job_requisitions",
                column: "business_unit_id",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
