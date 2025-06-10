using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class modifiedemailTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmailTemplateId",
                table: "email_template_variables",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_template_variables_EmailTemplateId",
                table: "email_template_variables",
                column: "EmailTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_email_template_variables_email_template_EmailTemplateId",
                table: "email_template_variables",
                column: "EmailTemplateId",
                principalTable: "email_template",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_email_template_variables_email_template_EmailTemplateId",
                table: "email_template_variables");

            migrationBuilder.DropIndex(
                name: "IX_email_template_variables_EmailTemplateId",
                table: "email_template_variables");

            migrationBuilder.DropColumn(
                name: "EmailTemplateId",
                table: "email_template_variables");
        }
    }
}
