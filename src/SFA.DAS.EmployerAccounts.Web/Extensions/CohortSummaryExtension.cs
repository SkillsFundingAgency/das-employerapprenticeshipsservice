using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public static class CohortSummaryExtension
{
    public static CohortStatus GetStatus(this CohortSummary cohort)
    {
        if (cohort.IsDraft && cohort.WithParty == Party.Employer)
            return CohortStatus.Draft;
        else if (!cohort.IsDraft && cohort.WithParty == Party.Employer)
            return CohortStatus.Review;
        else if (!cohort.IsDraft && cohort.WithParty == Party.Provider)
            return CohortStatus.WithTrainingProvider;
        else if (!cohort.IsDraft && cohort.WithParty == Party.TransferSender)
            return CohortStatus.WithTransferSender;
        else
            return CohortStatus.Unknown;
    }
}

public enum CohortStatus
{
    Unknown,
    Draft,
    Review,
    WithTrainingProvider,
    WithTransferSender
}