using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Web.Helpers;

public static class AccountTaskHelper
{
    public static int GetTaskPriority(AccountTask task)
    {
        return task.Type switch
        {
            "LevyDeclarationDue" => 1,
            "AgreementToSign" => 2,
            "AddApprentices" => 3,
            "ApprenticeChangesToReview" => 4,
            "CohortRequestReadyForApproval" => 5,
            "IncompleteApprenticeshipDetails" => 6,
            "ReviewConnectionRequest" => 7,
            "TransferRequestReceived" => 8,
            _ => int.MaxValue //if its an usupported type we place it last
        };
    }
}