using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_skills_skills_skill_id",
                table: "job_skills");

            migrationBuilder.AddForeignKey(
                name: "fk_jobskills_skill_id",
                table: "job_skills",
                column: "skill_id",
                principalTable: "skills",
                principalColumn: "skill_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_jobskills_skill_id",
                table: "job_skills");

            migrationBuilder.AddForeignKey(
                name: "FK_job_skills_skills_skill_id",
                table: "job_skills",
                column: "skill_id",
                principalTable: "skills",
                principalColumn: "skill_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
