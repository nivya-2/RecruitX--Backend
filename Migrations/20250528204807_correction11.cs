using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class correction11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_Department_DepartmentId",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "employees",
                newName: "department_id");

            migrationBuilder.RenameIndex(
                name: "IX_employees_DepartmentId",
                table: "employees",
                newName: "IX_employees_department_id");

            migrationBuilder.AddForeignKey(
                name: "fk_employees_department_id",
                table: "employees",
                column: "department_id",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employees_department_id",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "employees",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_department_id",
                table: "employees",
                newName: "IX_employees_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_Department_DepartmentId",
                table: "employees",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
