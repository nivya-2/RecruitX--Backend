using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_job_skills_id",
                table: "job_skills");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "assignment_id",
                table: "jr_assignments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "jd_id",
                table: "job_descriptions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "interview_id",
                table: "interviews",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "group_id",
                table: "interviewer_group",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "candidate_id",
                table: "candidates",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "application_id",
                table: "applications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "application_skill_id",
                table: "application_skills",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "interview_panel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "Id",
                table: "job_skills",
                column: "job_skills_id");

            migrationBuilder.CreateTable(
                name: "application_status_history",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    application_id = table.Column<int>(type: "integer", nullable: false),
                    old_status_id = table.Column<int>(type: "integer", nullable: true),
                    new_status_id = table.Column<int>(type: "integer", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    changed_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_status_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_application_status_history_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_status_history_users_changed_by",
                        column: x => x.changed_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "email_template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserType = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_template", x => x.Id);
                    table.ForeignKey(
                        name: "FK_email_template_users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "evaluation_tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    interview_id = table.Column<int>(type: "integer", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_evaluation_tokens_interviews_interview_id",
                        column: x => x.interview_id,
                        principalTable: "interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lead_to_recruiter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    RecruiterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_to_recruiter", x => new { x.Id, x.RecruiterId });
                    table.ForeignKey(
                        name: "FK_lead_to_recruiter_users_Id",
                        column: x => x.Id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lead_to_recruiter_users_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "email_template_variables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    template_id = table.Column<int>(type: "integer", nullable: false),
                    variable_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_template_variables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_email_template_variables_email_template_template_id",
                        column: x => x.template_id,
                        principalTable: "email_template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_status_history_application_id",
                table: "application_status_history",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_status_history_changed_by",
                table: "application_status_history",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "IX_email_template_CreatedBy",
                table: "email_template",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_email_template_variables_template_id_variable_name",
                table: "email_template_variables",
                columns: new[] { "template_id", "variable_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_tokens_interview_id",
                table: "evaluation_tokens",
                column: "interview_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_tokens_token",
                table: "evaluation_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lead_to_recruiter_RecruiterId",
                table: "lead_to_recruiter",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserId",
                table: "notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_status_history");

            migrationBuilder.DropTable(
                name: "email_template_variables");

            migrationBuilder.DropTable(
                name: "evaluation_tokens");

            migrationBuilder.DropTable(
                name: "lead_to_recruiter");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "email_template");

            migrationBuilder.DropPrimaryKey(
                name: "Id",
                table: "job_skills");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "interview_panel");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "jr_assignments",
                newName: "assignment_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "job_descriptions",
                newName: "jd_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "interviews",
                newName: "interview_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "interviewer_group",
                newName: "group_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "candidates",
                newName: "candidate_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "applications",
                newName: "application_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "application_skills",
                newName: "application_skill_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_job_skills_id",
                table: "job_skills",
                column: "job_skills_id");
        }
    }
}
