// AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using RecruitX.Models;
using RecruitX.Data;
using static JobSkill;
using RecruitX.Models.Config;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Client> Clients { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<JobRequisition> JobRequisitions { get; set; }
    
    public DbSet<JobDescription> JobDescriptions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<OnSiteDetail> OnSiteDetails { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }

    public DbSet<ApplicationSkill> ApplicationSkills { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<EmailTemplateVariable> EmailTemplateVariables { get; set; }
    public DbSet<EvaluationToken> EvaluationTokens { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<InterviewerGroup> InterviewerGroups { get; set; }
    public DbSet<InterviewPanel> InterviewPanels { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }
    public DbSet<JrAssignment> JrAssignments { get; set; }
    public DbSet<LeadToRecruiter> LeadToRecruiters { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<PanelToGroup> PanelToGroups { get; set; }

    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);


    }

}
