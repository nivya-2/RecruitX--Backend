using RecruitX.Data;

namespace RecruitX.Models.DTO
{
    public class CandidateDetailsDTO
    {
        public int? CandidateID {  get; set; }
        public int? ApplicationID {  get; set; }
        public string CandidateName { get; set; }
        public long CandidatePhone { get; set; }
        public string CandidateEmail { get; set; }
        public short TotalExperience { get; set; }
        public string subSource { get; set; }

        public string? linkedin {  get; set; }
        public string? preferedLocation {  get; set; }
        public string? role { get; set; }
        public short RelavantExperience { get; set; }

        public string? skill {  get; set; }

        public int? NoticePeriod { get; set; }
         public long CurrentCTC { get; set; }
        public long? ExpectedCTC  { get; set; }

        public string Source { get; set; }

        public string Status { get; set; } = "Applied";
        public string? JrStatus { get; set; } = "Open";
        public string CurrentLocation {  get; set; }
        public string CurrentEmployer { get; set; }
        public override string ToString()
        {
            return $"CandidateDetailsDTO: " +
                   $"CandidateID={CandidateID}, " +
                   $"ApplicationID={ApplicationID}, " +
                   $"CandidateName={CandidateName}, " +
                   $"CandidatePhone={CandidatePhone}, " +
                   $"CandidateEmail={CandidateEmail}, " +
                   $"TotalExperience={TotalExperience}, " +
                   $"RelavantExperience={RelavantExperience}, " +
                   $"Source={Source}, " +
                   $"SubSource={subSource}, " +
                   $"Skill={skill}, " +
                   $"Role={role}, " +
                   $"NoticePeriod={NoticePeriod}, " +
                   $"CurrentCTC={CurrentCTC}, " +
                   $"ExpectedCTC={ExpectedCTC}, " +
                   $"Status={Status}, " +
                   $"JrStatus={JrStatus}, " +
                   $"CurrentLocation={CurrentLocation}, " +
                   $"CurrentEmployer={CurrentEmployer}, " +
                   $"LinkedIn={linkedin}, " +
                   $"PreferredLocation={preferedLocation}";
        }


    }
}
