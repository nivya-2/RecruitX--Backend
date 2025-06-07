using Microsoft.Graph.Models;

namespace RecruitX.Models.DTO
{
    public class JobDescriptionDTO
    {
        public int JobRequisitionId { get; set; }

        /// <summary>
        /// The ID of the Job Description entity, if it exists.
        /// </summary>
        public int? JobDescriptionId { get; set; }

        // --- Role & Location ---

        /// <summary>
        /// The title of the role. Sourced from JobRequisition.Role.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The user-friendly work location string (e.g., "Bangalore, India").
        /// Populated by joining JobRequisition with the Location table.
        /// </summary>
        public string WorkLocation { get; set; }

        // --- Experience ---

        /// <summary>
        /// Required years of total experience. Sourced from JobRequisition.TotalExperienceYears.
        /// </summary>
        public int? TotalExperienceYears { get; set; }

        /// <summary>
        /// Required months of total experience. Will be set to 0 as per requirements.
        /// </summary>
        public int? TotalExperienceMonths { get; set; }

        /// <summary>
        /// Required years of relevant experience. Sourced from JobRequisition.RelevantExperienceYears.
        /// </summary>
        public int? RelevantExperienceYears { get; set; }

        /// <summary>
        /// Required months of relevant experience. Will be set to 0 as per requirements.
        /// </summary>
        public int? RelevantExperienceMonths { get; set; }

        // --- Skills & Qualifications ---

        /// <summary>
        /// The required educational qualifications. Sourced from JobRequisition.Qualification.
        /// </summary>
        public string Qualification { get; set; }

        /// <summary>
        //* Comma-separated string of mandatory skills.
        //* Populated by querying and aggregating the JobSkills table.
        /// </summary>
        public string SkillsMandatory { get; set; }

        /// <summary>
        //* Comma-separated string of primary skills.
        //* Populated by querying and aggregating the JobSkills table.
        /// </summary>
        public string SkillsPrimary { get; set; }

        /// <summary>
        //* Comma-separated string of "good to have" skills.
        //* Populated by querying and aggregating the JobSkills table.
        ///// </summary>
        public string SkillsGood { get; set; }

        // --- JD Content ---

        /// <summary>
        /// The main responsibilities/body of the job description. Sourced from JobDescription.JobDesc.
        /// </summary>
        public string JobDescription { get; set; }

        /// <summary>
        /// The high-level purpose of the job role. Sourced from JobRequisition.JobPurpose.
        /// </summary>
        public string JobPurpose { get; set; }

        /// <summary>
        /// Detailed specifications and technical requirements for the job. Sourced from JobRequisition.JobSpecification.
        /// </summary>
        public string JobSpecification { get; set; }

        // --- Dates ---

        /// <summary>
        /// The expected start date for the new hire, formatted as dd/MM/yyyy.
        /// Sourced from JobRequisition.ExpectedOnboardingDate.
        /// </summary>
        public string OnboardingDate { get; set; }

        public string? AdditionalInfo { get; set; }
    }
}

