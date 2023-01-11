using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

public class Apprenticeship
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CourseName { get; set; }
    public DateTime? CourseStartDate { get; set; }
    public DateTime? CourseEndDate { get; set; }
    public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
    public TrainingProvider TrainingProvider { get; private set; }
    public void SetTrainingProvider(TrainingProvider trainingProvider)
    {           
        TrainingProvider = trainingProvider;
    }
    public Cohort Cohort { get; private set; }
    public void SetCohort(Cohort cohort)
    {
        Cohort = cohort;
    }
    public string HashedId { get; private set; }
    public void SetHashId(IEncodingService encodingService)
    {
        HashedId = encodingService.Encode(Id, EncodingType.ApprenticeshipId);
    }
}

public enum ApprenticeshipStatus
{
    Draft = 0,
    Approved = 1
}