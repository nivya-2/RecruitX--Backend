using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_descriptions_employees_CreatedByEmployeeEmployeeId",
                table: "job_descriptions");

            migrationBuilder.RenameColumn(
                name: "JobRequisitionId",
                table: "job_requisitions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreatedByEmployeeEmployeeId",
                table: "job_descriptions",
                newName: "CreatedByEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_job_descriptions_CreatedByEmployeeEmployeeId",
                table: "job_descriptions",
                newName: "IX_job_descriptions_CreatedByEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_job_descriptions_employees_CreatedByEmployeeId",
                table: "job_descriptions",
                column: "CreatedByEmployeeId",
                principalTable: "employees",
                principalColumn: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_descriptions_employees_CreatedByEmployeeId",
                table: "job_descriptions");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "job_requisitions",
                newName: "JobRequisitionId");

            migrationBuilder.RenameColumn(
                name: "CreatedByEmployeeId",
                table: "job_descriptions",
                newName: "CreatedByEmployeeEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_job_descriptions_CreatedByEmployeeId",
                table: "job_descriptions",
                newName: "IX_job_descriptions_CreatedByEmployeeEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_job_descriptions_employees_CreatedByEmployeeEmployeeId",
                table: "job_descriptions",
                column: "CreatedByEmployeeEmployeeId",
                principalTable: "employees",
                principalColumn: "employee_id");
        }
    }
}
