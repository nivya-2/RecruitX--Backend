using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class rolesandcanddidatesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "candidates",
                columns: table => new
                {
                    candidate_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sub_source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    candidate_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    proposed_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contact_no = table.Column<long>(type: "bigint", nullable: false),
                    linkedin_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    total_experience_required_years = table.Column<short>(type: "smallint", nullable: false),
                    total_experience_required_months = table.Column<short>(type: "smallint", nullable: false),
                    relevant_experience_required_years = table.Column<short>(type: "smallint", nullable: false),
                    relevant_experience_required_months = table.Column<short>(type: "smallint", nullable: false),
                    current_employer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    current_location_id = table.Column<int>(type: "integer", nullable: true),
                    preferred_location_id = table.Column<int>(type: "integer", nullable: true),
                    notice_period_days = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidates", x => x.candidate_id);
                    table.ForeignKey(
                        name: "FK_candidates_locations_current_location_id",
                        column: x => x.current_location_id,
                        principalTable: "locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_candidates_locations_preferred_location_id",
                        column: x => x.preferred_location_id,
                        principalTable: "locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_candidates_current_location_id",
                table: "candidates",
                column: "current_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_candidates_email",
                table: "candidates",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_candidates_preferred_location_id",
                table: "candidates",
                column: "preferred_location_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "candidates");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
