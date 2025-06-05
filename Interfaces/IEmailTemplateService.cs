using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<List<EmailTemplateDto>> GetAllTemplatesAsync();
        Task<EmailTemplateDto?> GetTemplateByIdAsync(int id);
    }
}
