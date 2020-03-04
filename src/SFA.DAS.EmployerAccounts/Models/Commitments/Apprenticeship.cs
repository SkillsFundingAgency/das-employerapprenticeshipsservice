using SFA.DAS.Encoding;
using System;

namespace SFA.DAS.EmployerAccounts.Models.Commitments
{
    public class Apprenticeship
    {
        public long Id { get; set; }               
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public DateTime? CourseEndDate { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public TrainingProvider TrainingProvider { get; set; }
        public CohortV2 Cohort { get; private set; }
        public void SetCohort(CohortV2 cohort)
        {
            Cohort = cohort;
        }
        public string HashedId { get; private set; }
        public void SetHashId(IEncodingService encodingService)
        {
            HashedId = encodingService.Encode(Id, EncodingType.ApprenticeshipId);
        }
    }

    public class TrainingProvider
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public enum ApprenticeshipStatus
    {
        Draft = 0,
        Approved = 1
    }
}
