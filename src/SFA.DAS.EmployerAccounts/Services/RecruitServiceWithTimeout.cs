using SFA.DAS.EmployerAccounts.Models.Recruit;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Exceptions;

namespace SFA.DAS.EmployerAccounts.Services;

public class RecruitServiceWithTimeout : IRecruitService
{
    private readonly IRecruitService _recruitService;
    private readonly IAsyncPolicy _pollyPolicy;

    public RecruitServiceWithTimeout(
        IRecruitService recruitService, IReadOnlyPolicyRegistry<string> pollyRegistry)
    {
        _recruitService = recruitService;
        _pollyPolicy = pollyRegistry.Get<IAsyncPolicy>(Constants.DefaultServiceTimeout);
    }

    public async Task<IEnumerable<Vacancy>> GetVacancies(string hashedAccountId, int maxVacanciesToGet = int.MaxValue)
    {
        try
        {
            return await _pollyPolicy.ExecuteAsync(() => _recruitService.GetVacancies(hashedAccountId, maxVacanciesToGet));
        }
        catch (TimeoutRejectedException ex)
        {
            throw new ServiceTimeoutException("Call to Recruit Service timed out", ex);
        }
    }
}