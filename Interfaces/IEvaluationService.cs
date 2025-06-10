using RecruitX.Models.DTO;

namespace RecruitX.Interfaces
{
    public interface IEvaluationService
    {
        /// <summary>
        /// Generates a unique evaluation link for a specific interview, saves it to the database,
        /// and returns the full URL.
        /// </summary>
        /// <param name="interviewId">A unique identifier for the interview session.</param>
        /// <returns>The full one-time evaluation URL (e.g., "https://yourdomain.com/eval-form?token=...").</returns>
        Task<string> CreateEvaluationLinkAsync(int interviewId);

        /// <summary>
        /// Validates a token from an evaluation link to see if the form can be displayed.
        /// </summary>
        /// <param name="token">The unique token from the URL.</param>
        /// <returns>An DTO with context if the token is valid and pending, otherwise null.</returns>
        Task<EvaluationFormPocDto> GetEvaluationFormDetailsAsync(string token);

        /// <summary>
        /// Submits the feedback from the evaluation form, saves it, and marks the link as used.
        /// </summary>
        /// <param name="submissionDto">The DTO containing the token and the feedback.</param>
        /// <returns>True if the submission was successful, false otherwise.</returns>
        Task<bool> SubmitEvaluationAsync(SubmitEvaluationDto submissionDto);
        Task<ViewEvaluationDto> GetSubmittedEvaluationAsync(int interviewId);
    }
}
