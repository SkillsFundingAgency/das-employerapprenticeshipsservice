namespace SFA.DAS.EmployerAccounts.TasksApi;

public enum TaskType
{
    None = 0,
    LevyDeclarationDue = 1,
    AgreementToSign = 2,
    AddApprentices = 3,
    ApprenticeChangesToReview = 4,
    CohortRequestReadyForApproval = 5,
    IncompleteApprenticeshipDetails = 6,
    ReviewConnectionRequest = 7,
    TransferRequestReceived = 8
}