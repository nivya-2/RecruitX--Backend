using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_panel_to_group_interviewer_group_group_id",
                table: "panel_to_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group");

            migrationBuilder.DropIndex(
                name: "IX_panel_to_group_group_id_employee_id",
                table: "panel_to_group");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "panel_to_group",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "group_id",
                table: "panel_to_group",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "interview_panel",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "employees",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "panel_to_group",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "panel_to_group",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_panel_to_group_Id_employee_id",
                table: "panel_to_group",
                columns: new[] { "Id", "employee_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_panel_to_group_interviewer_group_Id",
                table: "panel_to_group",
                column: "Id",
                principalTable: "interviewer_group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_panel_to_group_interviewer_group_Id",
                table: "panel_to_group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group");

            migrationBuilder.DropIndex(
                name: "IX_panel_to_group_Id_employee_id",
                table: "panel_to_group");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "panel_to_group",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "panel_to_group",
                newName: "group_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "interview_panel",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "employees",
                newName: "employee_id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "panel_to_group",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "group_id",
                table: "panel_to_group",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_panel_to_group",
                table: "panel_to_group",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_panel_to_group_group_id_employee_id",
                table: "panel_to_group",
                columns: new[] { "group_id", "employee_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_panel_to_group_interviewer_group_group_id",
                table: "panel_to_group",
                column: "group_id",
                principalTable: "interviewer_group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
