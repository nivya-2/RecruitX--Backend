using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class jd_status1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Jd_status",
                table: "job_descriptions");

            migrationBuilder.AddColumn<string>(
                name: "Jd_status",
                table: "job_requisitions",
                type: "text",
                nullable: false,
                defaultValue: "GenerateJD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Jd_status",
                table: "job_requisitions");

            migrationBuilder.AddColumn<string>(
                name: "Jd_status",
                table: "job_descriptions",
                type: "text",
                nullable: false,
                defaultValue: "GenerateJD");
        }
    }
}
