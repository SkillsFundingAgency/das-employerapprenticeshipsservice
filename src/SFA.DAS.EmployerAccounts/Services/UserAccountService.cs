using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.UserAccounts;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.UserAccounts;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Models.UserAccounts;

namespace SFA.DAS.EmployerAccounts.Services;

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
