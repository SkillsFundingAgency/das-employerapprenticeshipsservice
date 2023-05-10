using SFA.DAS.EAS.Application.Contracts.OuterApi;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Application.Infrastructure.OuterApi.Requests;
using SFA.DAS.EAS.Application.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.EAS.Web.Services;

public interface IUserAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}

public class UserAccountService : IUserAccountService
{
    private readonly IOuterApiClient _outerApiClient;

    public UserAccountService(IOuterApiClient outerApiClient)
    {
        _outerApiClient = outerApiClient;
    }
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var actual = await _outerApiClient.Get<GetUserAccountsResponse>(new GetUserAccountsRequest(email, userId));

        return actual;
    }
}
