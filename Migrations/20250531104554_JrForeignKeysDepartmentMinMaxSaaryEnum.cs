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
            migrationBuilder.DropColumn(
                name: "BusinessUnit",
                table: "job_requisitions");

            migrationBuilder.DropColumn(
                name: "ExpectedSalaryRange",
                table: "job_requisitions");

            migrationBuilder.AddColumn<int>(
                name: "ExpectedSalaryMaximum",
                table: "job_requisitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedSalaryMinimum",
                table: "job_requisitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "job_requisitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_business_unit_id",
                table: "job_requisitions",
                column: "business_unit_id");

            migrationBuilder.AddForeignKey(
                name: "FK_job_requisitions_departments_business_unit_id",
                table: "job_requisitions",
                column: "business_unit_id",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_requisitions_departments_business_unit_id",
                table: "job_requisitions");

            migrationBuilder.DropIndex(
                name: "IX_job_requisitions_business_unit_id",
                table: "job_requisitions");

            migrationBuilder.DropColumn(
                name: "ExpectedSalaryMaximum",
                table: "job_requisitions");

            migrationBuilder.DropColumn(
                name: "ExpectedSalaryMinimum",
                table: "job_requisitions");

            migrationBuilder.DropColumn(
                name: "business_unit_id",
                table: "job_requisitions");

            migrationBuilder.AddColumn<string>(
                name: "BusinessUnit",
                table: "job_requisitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpectedSalaryRange",
                table: "job_requisitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
