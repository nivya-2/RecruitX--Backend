using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_descriptions_employees_CreatedByEmployeeId",
                table: "job_descriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_job_descriptions_employees_created_by",
                table: "job_descriptions");

            migrationBuilder.DropIndex(
                name: "IX_job_descriptions_CreatedByEmployeeId",
                table: "job_descriptions");

            migrationBuilder.DropColumn(
                name: "CreatedByEmployeeId",
                table: "job_descriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_job_descriptions_users_created_by",
                table: "job_descriptions",
                column: "created_by",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_descriptions_users_created_by",
                table: "job_descriptions");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByEmployeeId",
                table: "job_descriptions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_descriptions_CreatedByEmployeeId",
                table: "job_descriptions",
                column: "CreatedByEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_job_descriptions_employees_CreatedByEmployeeId",
                table: "job_descriptions",
                column: "CreatedByEmployeeId",
                principalTable: "employees",
                principalColumn: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "FK_job_descriptions_employees_created_by",
                table: "job_descriptions",
                column: "created_by",
                principalTable: "employees",
                principalColumn: "employee_id");
        }
    }
}
