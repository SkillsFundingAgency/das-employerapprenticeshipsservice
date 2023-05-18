using SFA.DAS.EmployerAccounts.TasksApi;

namespace SFA.DAS.EmployerAccounts.Web.Helpers;

public static class AccountTaskHelper
{
    public static int GetTaskPriority(AccountTask task)
    {
        switch (task.Type)
        {
            case nameof(TaskType.LevyDeclarationDue): return 1;
            case nameof(TaskType.AgreementToSign): return 2;
            case nameof(TaskType.AddApprentices): return 3;
            case nameof(TaskType.ApprenticeChangesToReview): return 4;
            case nameof(TaskType.CohortRequestReadyForApproval): return 5;
            case nameof(TaskType.IncompleteApprenticeshipDetails): return 6;
            case nameof(TaskType.ReviewConnectionRequest): return 7;
            case nameof(TaskType.TransferRequestReceived): return 8;

            default: return int.MaxValue; //if its an unsupported type we place it last
        }
    }
}