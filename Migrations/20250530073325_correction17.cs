using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interview_panel_employees_EmployeeId",
                table: "interview_panel");

            migrationBuilder.DropForeignKey(
                name: "FK_interview_panel_interviews_InterviewId",
                table: "interview_panel");

            migrationBuilder.DropForeignKey(
                name: "FK_panel_to_group_employees_EmployeeId",
                table: "panel_to_group");

            migrationBuilder.DropForeignKey(
                name: "FK_panel_to_group_interviewer_group_GroupId",
                table: "panel_to_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_interview_panel",
                table: "interview_panel");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "skill_id",
                table: "skills",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "panel_to_group",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "panel_to_group",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "panel_to_group",
                newName: "group_id");

            migrationBuilder.RenameIndex(
                name: "IX_panel_to_group_EmployeeId",
                table: "panel_to_group",
                newName: "IX_panel_to_group_employee_id");

            migrationBuilder.RenameColumn(
                name: "onsite_detail_id",
                table: "on_site_details",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "location_id",
                table: "locations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "job_skills_id",
                table: "job_skills",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "interview_panel",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "interview_panel",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "InterviewId",
                table: "interview_panel",
                newName: "interview_id");

            migrationBuilder.RenameIndex(
                name: "IX_interview_panel_EmployeeId",
                table: "interview_panel",
                newName: "IX_interview_panel_employee_id");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "Clients",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "panel_to_group",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "interview_panel",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_interview_panel",
                table: "interview_panel",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_panel_to_group_group_id_employee_id",
                table: "panel_to_group",
                columns: new[] { "group_id", "employee_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_interview_panel_interview_id_employee_id",
                table: "interview_panel",
                columns: new[] { "interview_id", "employee_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_interview_panel_employees_employee_id",
                table: "interview_panel",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_interview_panel_interviews_interview_id",
                table: "interview_panel",
                column: "interview_id",
                principalTable: "interviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_panel_to_group_employees_employee_id",
                table: "panel_to_group",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_panel_to_group_interviewer_group_group_id",
                table: "panel_to_group",
                column: "group_id",
                principalTable: "interviewer_group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interview_panel_employees_employee_id",
                table: "interview_panel");

            migrationBuilder.DropForeignKey(
                name: "FK_interview_panel_interviews_interview_id",
                table: "interview_panel");

            migrationBuilder.DropForeignKey(
                name: "FK_panel_to_group_employees_employee_id",
                table: "panel_to_group");

            migrationBuilder.DropForeignKey(
                name: "FK_panel_to_group_interviewer_group_group_id",
                table: "panel_to_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group");

            migrationBuilder.DropIndex(
                name: "IX_panel_to_group_group_id_employee_id",
                table: "panel_to_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_interview_panel",
                table: "interview_panel");

            migrationBuilder.DropIndex(
                name: "IX_interview_panel_interview_id_employee_id",
                table: "interview_panel");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "skills",
                newName: "skill_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "panel_to_group",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "group_id",
                table: "panel_to_group",
                newName: "GroupId");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "panel_to_group",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_panel_to_group_employee_id",
                table: "panel_to_group",
                newName: "IX_panel_to_group_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "on_site_details",
                newName: "onsite_detail_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "locations",
                newName: "location_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "job_skills",
                newName: "job_skills_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "interview_panel",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "interview_id",
                table: "interview_panel",
                newName: "InterviewId");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "interview_panel",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_interview_panel_employee_id",
                table: "interview_panel",
                newName: "IX_interview_panel_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Clients",
                newName: "client_id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "panel_to_group",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "interview_panel",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group",
                columns: new[] { "GroupId", "EmployeeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_interview_panel",
                table: "interview_panel",
                columns: new[] { "InterviewId", "EmployeeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_interview_panel_employees_EmployeeId",
                table: "interview_panel",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_interview_panel_interviews_InterviewId",
                table: "interview_panel",
                column: "InterviewId",
                principalTable: "interviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_panel_to_group_employees_EmployeeId",
                table: "panel_to_group",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_panel_to_group_interviewer_group_GroupId",
                table: "panel_to_group",
                column: "GroupId",
                principalTable: "interviewer_group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
