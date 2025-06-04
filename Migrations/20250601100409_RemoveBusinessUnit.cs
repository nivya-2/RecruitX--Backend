using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBusinessUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "BusinessUnit",
               table: "job_requisitions");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
