using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Exceptions;

namespace SFA.DAS.EmployerAccounts.Services;

public class CommitmentsV2ServiceWithTimeout : ICommitmentV2Service
{
    private readonly ICommitmentV2Service _commitmentsV2Service;
    private readonly IAsyncPolicy _pollyPolicy;

    public CommitmentsV2ServiceWithTimeout(ICommitmentV2Service commitmentsV2Service,
        IReadOnlyPolicyRegistry<string> pollyRegistry)
    {
        _commitmentsV2Service = commitmentsV2Service;
        _pollyPolicy = pollyRegistry.Get<IAsyncPolicy>(Constants.DefaultServiceTimeout);
    }

    public async Task<IEnumerable<Apprenticeship>> GetDraftApprenticeships(Cohort cohort)
    {
        try
        {
            return await _pollyPolicy.ExecuteAsync(() => _commitmentsV2Service.GetDraftApprenticeships(cohort));
        }
        catch (TimeoutRejectedException ex)
        {
            throw new ServiceTimeoutException("Call to Commitments V2 Service timed out", ex);
        }
    }

    public async Task<IEnumerable<Cohort>> GetCohorts(long? accountId)
    {
        try
        {
            return await _pollyPolicy.ExecuteAsync(() => _commitmentsV2Service.GetCohorts(accountId));
        }
        catch (TimeoutRejectedException ex)
        {
            throw new ServiceTimeoutException("Call to Commitments V2 Service timed out", ex);
        }
    }

    public async Task<IEnumerable<Apprenticeship>> GetApprenticeships(long accountId)
    {
        try
        {
            return await _pollyPolicy.ExecuteAsync(() => _commitmentsV2Service.GetApprenticeships(accountId));
        }
        catch (TimeoutRejectedException ex)
        {
            throw new ServiceTimeoutException("Call to Commitments V2 Service timed out", ex);
        }
    }

    public async Task<List<Cohort>> GetEmployerCommitments(long employerAccountId)
    {
        try
        {
            return await _pollyPolicy.ExecuteAsync(() => _commitmentsV2Service.GetEmployerCommitments(employerAccountId));
        }
        catch (TimeoutRejectedException ex)
        {
            throw new ServiceTimeoutException("Call to Commitments V2 Service timed out", ex);
        }
    }
}