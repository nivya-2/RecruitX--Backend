using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitX.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_name = table.Column<string>(type: "text", nullable: false),
                    client_country = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    location_name = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.location_id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    skill_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    skill_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.skill_id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<long>(type: "bigint", nullable: true),
                    position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    delivery_unit = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    location_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.employee_id);
                    table.ForeignKey(
                        name: "FK_employees_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_requisitions",
                columns: table => new
                {
                    JobRequisitionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestedBy = table.Column<int>(type: "integer", nullable: true),
                    HiringManager = table.Column<int>(type: "integer", nullable: true),
                    NumPositions = table.Column<int>(type: "integer", nullable: true),
                    WorkShift = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExpectedOnboardingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WorkModel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Qualification = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    JobDuties = table.Column<string>(type: "text", nullable: false),
                    TotalExperienceYears = table.Column<int>(type: "integer", nullable: true),
                    TotalExperienceMonths = table.Column<int>(type: "integer", nullable: true),
                    RelevantExperienceYears = table.Column<int>(type: "integer", nullable: true),
                    RelevantExperienceMonths = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    JobPurpose = table.Column<string>(type: "text", nullable: false),
                    JobSpecification = table.Column<string>(type: "text", nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProjectRole = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OnsiteOpportunity = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Billable = table.Column<bool>(type: "boolean", nullable: true),
                    ClientInterview = table.Column<bool>(type: "boolean", nullable: true),
                    ClientId = table.Column<int>(type: "integer", nullable: true),
                    ExpectedSalaryRange = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdealStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JdStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_requisitions", x => x.JobRequisitionId);
                    table.ForeignKey(
                        name: "FK_job_requisitions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_job_requisitions_employees_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_job_requisitions_employees_HiringManager",
                        column: x => x.HiringManager,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_job_requisitions_employees_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_job_requisitions_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    employee_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_descriptions",
                columns: table => new
                {
                    jd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jr_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    job_desc = table.Column<string>(type: "text", nullable: true),
                    fill_positions = table.Column<int>(type: "integer", nullable: true),
                    updates = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    JobRequisitionId1 = table.Column<int>(type: "integer", nullable: true),
                    CreatedByEmployeeEmployeeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_descriptions", x => x.jd_id);
                    table.ForeignKey(
                        name: "FK_job_descriptions_employees_CreatedByEmployeeEmployeeId",
                        column: x => x.CreatedByEmployeeEmployeeId,
                        principalTable: "employees",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_job_descriptions_employees_created_by",
                        column: x => x.created_by,
                        principalTable: "employees",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_job_descriptions_job_requisitions_JobRequisitionId1",
                        column: x => x.JobRequisitionId1,
                        principalTable: "job_requisitions",
                        principalColumn: "JobRequisitionId");
                    table.ForeignKey(
                        name: "FK_job_descriptions_job_requisitions_jr_id",
                        column: x => x.jr_id,
                        principalTable: "job_requisitions",
                        principalColumn: "JobRequisitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_skills",
                columns: table => new
                {
                    job_skills_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jr_id = table.Column<int>(type: "integer", nullable: false),
                    skill_id = table.Column<int>(type: "integer", nullable: false),
                    skill_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "MANDATORY")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_skills_id", x => x.job_skills_id);
                    table.ForeignKey(
                        name: "FK_job_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_jobskills_jr_id",
                        column: x => x.jr_id,
                        principalTable: "job_requisitions",
                        principalColumn: "JobRequisitionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "on_site_details",
                columns: table => new
                {
                    onsite_detail_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jr_id = table.Column<int>(type: "integer", nullable: false),
                    rate = table.Column<string>(type: "text", nullable: false),
                    ideal_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    contract_type = table.Column<string>(type: "text", nullable: true),
                    contract_duration = table.Column<string>(type: "text", nullable: false),
                    reporting_to = table.Column<string>(type: "text", nullable: false),
                    preferred_time_zone = table.Column<string>(type: "text", nullable: false),
                    preferred_visa_status = table.Column<string>(type: "text", nullable: false),
                    h1_transfer_accepted = table.Column<bool>(type: "boolean", nullable: true),
                    interview_process = table.Column<string>(type: "text", nullable: true),
                    travel_required = table.Column<bool>(type: "boolean", nullable: true),
                    client_background = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    WorkLocation = table.Column<string>(type: "text", nullable: false),
                    ClientLocation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_on_site_details", x => x.onsite_detail_id);
                    table.ForeignKey(
                        name: "FK_OnsiteJobDetail_JobRequisition",
                        column: x => x.jr_id,
                        principalTable: "job_requisitions",
                        principalColumn: "JobRequisitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jr_assignments",
                columns: table => new
                {
                    assignment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    jr_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_to = table.Column<int>(type: "integer", nullable: false),
                    assigned_by = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jr_assignments", x => x.assignment_id);
                    table.ForeignKey(
                        name: "FK_jr_assignments_job_requisitions_jr_id",
                        column: x => x.jr_id,
                        principalTable: "job_requisitions",
                        principalColumn: "JobRequisitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_jr_assignments_users_assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_jr_assignments_users_assigned_to",
                        column: x => x.assigned_to,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employees_location_id",
                table: "employees",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_descriptions_created_by",
                table: "job_descriptions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_job_descriptions_CreatedByEmployeeEmployeeId",
                table: "job_descriptions",
                column: "CreatedByEmployeeEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_job_descriptions_JobRequisitionId1",
                table: "job_descriptions",
                column: "JobRequisitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_job_descriptions_jr_id",
                table: "job_descriptions",
                column: "jr_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_ClientId",
                table: "job_requisitions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_CreatedBy",
                table: "job_requisitions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_HiringManager",
                table: "job_requisitions",
                column: "HiringManager");

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_LocationId",
                table: "job_requisitions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_job_requisitions_RequestedBy",
                table: "job_requisitions",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_job_skills_jr_id",
                table: "job_skills",
                column: "jr_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_skills_skill_id",
                table: "job_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_jr_assignments_assigned_by",
                table: "jr_assignments",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_jr_assignments_assigned_to",
                table: "jr_assignments",
                column: "assigned_to");

            migrationBuilder.CreateIndex(
                name: "IX_jr_assignments_jr_id",
                table: "jr_assignments",
                column: "jr_id");

            migrationBuilder.CreateIndex(
                name: "IX_on_site_details_jr_id",
                table: "on_site_details",
                column: "jr_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_skills_skill_name",
                table: "skills",
                column: "skill_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_employee_id",
                table: "users",
                column: "employee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_descriptions");

            migrationBuilder.DropTable(
                name: "job_skills");

            migrationBuilder.DropTable(
                name: "jr_assignments");

            migrationBuilder.DropTable(
                name: "on_site_details");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "job_requisitions");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "locations");
        }
    }
}
