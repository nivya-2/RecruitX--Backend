using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationEntities_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ideal_start_date",
                table: "on_site_details",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "NumPositions",
                table: "job_requisitions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "deleted_at",
            //    table: "job_requisitions",
            //    type: "timestamptz",
            //    nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "fill_positions",
                table: "job_descriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "ApplicationId1",
            //    table: "interviews",
            //    type: "integer",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "EmailTemplateId",
            //    table: "email_template_variables",
            //    type: "integer",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "PanelEvaluationLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    InterviewId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelEvaluationLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PanelEvaluationResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PanelEvaluationLinkId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Feedback = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelEvaluationResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PanelEvaluationResponses_PanelEvaluationLinks_PanelEvaluati~",
                        column: x => x.PanelEvaluationLinkId,
                        principalTable: "PanelEvaluationLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_interviews_ApplicationId1",
            //    table: "interviews",
            //    column: "ApplicationId1");

            //migrationBuilder.CreateIndex(
            //    name: "IX_email_template_variables_EmailTemplateId",
            //    table: "email_template_variables",
            //    column: "EmailTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PanelEvaluationResponses_PanelEvaluationLinkId",
                table: "PanelEvaluationResponses",
                column: "PanelEvaluationLinkId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_email_template_variables_email_template_EmailTemplateId",
            //    table: "email_template_variables",
            //    column: "EmailTemplateId",
            //    principalTable: "email_template",
            //    principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_interviews_applications_ApplicationId1",
            //    table: "interviews",
            //    column: "ApplicationId1",
            //    principalTable: "applications",
            //    principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This is correct - we want to be able to drop this foreign key
            //migrationBuilder.DropForeignKey(
            //    name: "FK_email_template_variables_email_template_EmailTemplateId",
            //    table: "email_template_variables");

            // =========================================================
            // This corresponds to the foreign key we commented out in the Up() method.
            // It MUST also be commented out.
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_interviews_applications_ApplicationId1",
                table: "interviews");
            */
            // =========================================================

            // This is correct
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users");

            // This is correct - we want to be able to drop our new tables
            migrationBuilder.DropTable(
                name: "PanelEvaluationResponses");

            migrationBuilder.DropTable(
                name: "PanelEvaluationLinks");

            // This is correct
            migrationBuilder.DropIndex(
                name: "IX_users_RoleId",
                table: "users");

            // =========================================================
            // This corresponds to the index we commented out in the Up() method.
            /*
            migrationBuilder.DropIndex(
                name: "IX_interviews_ApplicationId1",
                table: "interviews");
            */
            // =========================================================

            // This is correct
            //migrationBuilder.DropIndex(
            //    name: "IX_email_template_variables_EmailTemplateId",
            //    table: "email_template_variables");

            // This is correct
            //migrationBuilder.DropColumn(
            //    name: "deleted_at",
            //    table: "job_requisitions");

            // =========================================================
            // This corresponds to the column we commented out in the Up() method.
            /*
            migrationBuilder.DropColumn(
                name: "ApplicationId1",
                table: "interviews");
            */
            // =========================================================

            // This is correct
            //migrationBuilder.DropColumn(
            //    name: "EmailTemplateId",
            //    table: "email_template_variables");

            // The rest of the AlterColumn calls are correct
            migrationBuilder.AlterColumn<DateTime>(
                name: "ideal_start_date",
                table: "on_site_details",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

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
