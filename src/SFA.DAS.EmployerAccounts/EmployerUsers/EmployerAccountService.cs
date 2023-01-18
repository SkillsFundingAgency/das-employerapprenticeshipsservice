using SFA.DAS.EmployerAccounts.EmployerUsers.ApiResponse;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.EmployerUsers;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.EmployerUsers;
public interface IEmployerAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}

public class EmployerAccountService : IEmployerAccountService
{
    private readonly IOuterApiClient _apiClient;

    public EmployerAccountService(IOuterApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var actual = await _apiClient.Get<GetUserAccountsResponse>(new GetEmployerAccountsRequest(email, userId));

        return actual;
    }
}