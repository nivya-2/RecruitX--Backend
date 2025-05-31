using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OnSiteDetailId",
                table: "job_requisitions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_OnSiteDetailId",
                table: "job_requisitions",
                column: "OnSiteDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_job_requisitions_on_site_details_OnSiteDetailId",
                table: "job_requisitions",
                column: "OnSiteDetailId",
                principalTable: "on_site_details",
                principalColumn: "onsite_detail_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_requisitions_on_site_details_OnSiteDetailId",
                table: "job_requisitions");

            migrationBuilder.DropIndex(
                name: "IX_job_requisitions_OnSiteDetailId",
                table: "job_requisitions");

            migrationBuilder.DropColumn(
                name: "OnSiteDetailId",
                table: "job_requisitions");
        }
    }
}
