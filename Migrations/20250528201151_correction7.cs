using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "job_descriptions");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "job_requisitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "job_requisitions");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "job_descriptions",
                type: "text",
                nullable: true);
        }
    }
}
