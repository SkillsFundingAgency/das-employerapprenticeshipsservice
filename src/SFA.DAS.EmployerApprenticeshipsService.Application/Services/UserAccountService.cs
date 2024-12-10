using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Contracts.OuterApi;
using SFA.DAS.EAS.Application.Infrastructure.OuterApi.Requests;
using SFA.DAS.EAS.Application.Infrastructure.OuterApi.Responses;
using SFA.DAS.GovUK.Auth.Employer;

namespace SFA.DAS.EAS.Application.Services;

public interface IUserAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}

public class UserAccountService(IOuterApiClient outerApiClient) : IUserAccountService, IGovAuthEmployerAccountService
{
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var result = await outerApiClient.Get<GetUserAccountsResponse>(new GetUserAccountsRequest(email, userId));

        return new EmployerUserAccounts
        {
            EmployerAccounts = result.UserAccounts != null? result.UserAccounts.Select(c => new EmployerUserAccountItem
            {
                Role = c.Role,
                AccountId = c.AccountId,
                ApprenticeshipEmployerType = Enum.Parse<ApprenticeshipEmployerType>(c.ApprenticeshipEmployerType.ToString()),
                EmployerName = c.EmployerName,
            }).ToList() : [],
            FirstName = result.FirstName,
            IsSuspended = result.IsSuspended,
            LastName = result.LastName,
            EmployerUserId = result.EmployerUserId,
        };
    }
}
