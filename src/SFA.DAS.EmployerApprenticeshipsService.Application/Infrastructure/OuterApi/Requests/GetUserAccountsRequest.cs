using System;
using SFA.DAS.EAS.Application.Contracts.OuterApi;

namespace SFA.DAS.EAS.Application.Infrastructure.OuterApi.Requests;

public class GetUserAccountsRequest : IGetApiRequest
{
    private readonly string _email;
    private readonly string _userId;

    public GetUserAccountsRequest(string email, string userId)
    {
        _email = Uri.EscapeDataString(email);
        _userId = userId;
    }

    public string GetUrl => $"accountusers/{_userId}/accounts?email={_email}";
}