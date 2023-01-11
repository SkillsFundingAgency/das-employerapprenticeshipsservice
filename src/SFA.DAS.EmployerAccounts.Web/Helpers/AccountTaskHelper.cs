namespace SFA.DAS.EmployerAccounts.Web.Helpers;

public class AccountTaskHelper
{
    public static int GetTaskPriority(AccountTask task)
    {
        switch (task.Type)
        {
            case "LevyDeclarationDue": return 1;
            case "AgreementToSign": return 2;
            case "AddApprentices": return 3;
            case "ApprenticeChangesToReview": return 4;
            case "CohortRequestReadyForApproval": return 5;
            case "IncompleteApprenticeshipDetails": return 6;
            case "ReviewConnectionRequest": return 7;
            case "TransferRequestReceived": return 8;

            default: return int.MaxValue; //if its an usupported type we place it last
        }
    }
}