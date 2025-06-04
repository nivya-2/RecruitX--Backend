using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class updatestojr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
         

            migrationBuilder.DropColumn(
                name: "RelevantExperienceMonths",
                table: "job_requisitions");

            migrationBuilder.DropColumn(
                name: "TotalExperienceMonths",
                table: "job_requisitions");

            

            migrationBuilder.AlterColumn<string>(
                name: "WorkShift",
                table: "job_requisitions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "RequestedDate",
                table: "job_requisitions",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "IdealStartDate",
                table: "job_requisitions",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpectedOnboardingDate",
                table: "job_requisitions",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            

            

     
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
          

            migrationBuilder.AlterColumn<string>(
                name: "WorkShift",
                table: "job_requisitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestedDate",
                table: "job_requisitions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IdealStartDate",
                table: "job_requisitions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedOnboardingDate",
                table: "job_requisitions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelevantExperienceMonths",
                table: "job_requisitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalExperienceMonths",
                table: "job_requisitions",
                type: "integer",
                nullable: true);


            
        }
    }
}
