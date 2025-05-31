using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OnsiteOpportunity",
                table: "job_requisitions",
                newName: "HasOnsiteOpportunity");

            migrationBuilder.RenameColumn(
                name: "ClientInterview",
                table: "job_requisitions",
                newName: "IsBillable");

            migrationBuilder.RenameColumn(
                name: "Billable",
                table: "job_requisitions",
                newName: "HasClientInterview");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBillable",
                table: "job_requisitions",
                newName: "ClientInterview");

            migrationBuilder.RenameColumn(
                name: "HasOnsiteOpportunity",
                table: "job_requisitions",
                newName: "OnsiteOpportunity");

            migrationBuilder.RenameColumn(
                name: "HasClientInterview",
                table: "job_requisitions",
                newName: "Billable");
        }
    }
}
