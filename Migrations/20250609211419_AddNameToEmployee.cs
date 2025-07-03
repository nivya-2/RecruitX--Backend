using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NumPositions",
                table: "job_requisitions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "fill_positions",
                table: "job_descriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId1",
                table: "interviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Name",
                table: "employees");

            migrationBuilder.AlterColumn<int>(
                name: "NumPositions",
                table: "job_requisitions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "fill_positions",
                table: "job_descriptions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
