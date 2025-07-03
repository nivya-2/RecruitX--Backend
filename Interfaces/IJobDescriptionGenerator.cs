namespace RecruitX.Interfaces
{
    public interface IJobDescriptionGenerator
    {
        Task<string> GenerateJobDescriptionAsync(string prompt);

    }
}
