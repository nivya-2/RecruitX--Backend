using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_skills");

            migrationBuilder.DropTable(
                name: "applications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    application_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    candidate_id = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    jd_id = table.Column<int>(type: "integer", nullable: false),
                    experience_months = table.Column<short>(type: "smallint", nullable: false),
                    experience_years = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Applied"),
                    submitted_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.application_id);
                    table.CheckConstraint("CK_Application_ExperienceMonths", "experience_months >= 0 AND experience_months <= 11");
                    table.ForeignKey(
                        name: "fk_applications_candidate_id",
                        column: x => x.candidate_id,
                        principalTable: "candidates",
                        principalColumn: "candidate_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_applications_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_applications_jd_id",
                        column: x => x.jd_id,
                        principalTable: "job_descriptions",
                        principalColumn: "jd_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "application_skills",
                columns: table => new
                {
                    application_skill_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    application_id = table.Column<int>(type: "integer", nullable: false),
                    skill_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_skills", x => x.application_skill_id);
                    table.ForeignKey(
                        name: "fk_application_skills_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "application_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_application_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_skills_skill_id",
                table: "application_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "uq_application_skills_application_skill",
                table: "application_skills",
                columns: new[] { "application_id", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_applications_candidate_id_jd_id",
                table: "applications",
                columns: new[] { "candidate_id", "jd_id" });

            migrationBuilder.CreateIndex(
                name: "IX_applications_created_by",
                table: "applications",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_applications_jd_id",
                table: "applications",
                column: "jd_id");
        }
    }
}
