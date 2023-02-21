using System.Net;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.UserAccounts;

public class GetUserAccountsRequest : IGetApiRequest
{
    private readonly string _email;
    private readonly string _userId;

    public GetUserAccountsRequest(string email, string userId)
    {
        _email = WebUtility.UrlEncode(email);
        _userId = userId;
    }

    public string GetUrl => $"accountusers/{_userId}/accounts?email={_email}";
}