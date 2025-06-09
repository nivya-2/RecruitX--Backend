using Microsoft.EntityFrameworkCore;
using RecruitX.Interfaces;
using RecruitX.Models.DTO;

namespace RecruitX.Repositories
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly AppDbContext _context;

        public EmailTemplateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmailTemplateDto>> GetAllTemplatesAsync()
        {
            return await _context.EmailTemplates
                .Include(t => t.Creator)
                .Include(t => t.TemplateVariables)
                .Select(t => new EmailTemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Subject = t.Subject,
                    Body = t.Body,
                    UserType = t.UserType,
                    Variables = t.TemplateVariables.Select(v => v.VariableName).ToList()
                })
                .ToListAsync();
        }

        public async Task<EmailTemplateDto?> GetTemplateByIdAsync(int id)
        {
            return await _context.EmailTemplates
                .Include(t => t.TemplateVariables)
                .Where(t => t.Id == id)
                .Select(t => new EmailTemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Subject = t.Subject,
                    Body = t.Body,
                    UserType = t.UserType,
                    Variables = t.TemplateVariables.Select(v => v.VariableName).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }

}
