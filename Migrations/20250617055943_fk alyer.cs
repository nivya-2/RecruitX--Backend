using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class fkalyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_descriptions_job_requisitions_JobRequisitionId1",
                table: "job_descriptions");

            migrationBuilder.DropIndex(
                name: "IX_job_descriptions_JobRequisitionId1",
                table: "job_descriptions");

            migrationBuilder.DropColumn(
                name: "JobRequisitionId1",
                table: "job_descriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobRequisitionId1",
                table: "job_descriptions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_descriptions_JobRequisitionId1",
                table: "job_descriptions",
                column: "JobRequisitionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_job_descriptions_job_requisitions_JobRequisitionId1",
                table: "job_descriptions",
                column: "JobRequisitionId1",
                principalTable: "job_requisitions",
                principalColumn: "Id");
        }
    }
}
