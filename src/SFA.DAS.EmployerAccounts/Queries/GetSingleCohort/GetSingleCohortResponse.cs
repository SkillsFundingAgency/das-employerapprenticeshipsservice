using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

namespace SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;

public class GetSingleCohortResponse
{
    public Cohort Cohort { get; set; }
    public bool HasFailed { get; set; }
}