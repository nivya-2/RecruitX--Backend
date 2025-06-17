using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class applicationfk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interviews_applications_ApplicationId1",
                table: "interviews");

            migrationBuilder.DropIndex(
                name: "IX_interviews_ApplicationId1",
                table: "interviews");

            migrationBuilder.DropColumn(
                name: "ApplicationId1",
                table: "interviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId1",
                table: "interviews",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_interviews_ApplicationId1",
                table: "interviews",
                column: "ApplicationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_interviews_applications_ApplicationId1",
                table: "interviews",
                column: "ApplicationId1",
                principalTable: "applications",
                principalColumn: "Id");
        }
    }
}
