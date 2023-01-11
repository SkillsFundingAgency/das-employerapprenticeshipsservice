using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

public class Cohort
{
    public long Id { get; set; }

    public int? NumberOfDraftApprentices { get; set; }
    public CohortStatus CohortStatus { get; set; }
    public IEnumerable<Apprenticeship> Apprenticeships { get; set; }
    public string HashedId { get; private set; }
    public void SetHashId(IEncodingService encodingService)
    {
        HashedId = encodingService.Encode(Id, EncodingType.CohortReference);
    }
    public IEnumerable<TrainingProvider> TrainingProvider { get; private set; }
    public void SetTrainingProvider(IEnumerable<TrainingProvider> trainingProvider)
    {            
        TrainingProvider = trainingProvider;
    }
}

public enum CohortStatus
{
    Unknown,
    Draft,
    Review,
    WithTrainingProvider,
    WithTransferSender,
    Approved
}