using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface ICommitmentV2Service
{
    Task<IEnumerable<Cohort>> GetCohorts(long? accountId);        

    Task<IEnumerable<Apprenticeship>> GetDraftApprenticeships(Cohort cohort);

    Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId);

    Task<List<Cohort>> GetEmployerCommitments(long employerAccountId);
}