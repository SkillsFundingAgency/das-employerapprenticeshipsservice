using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;

public class GetUserAccountsQueryHandler : IRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>
{
    private readonly IUserAccountRepository _userAccountsRepository;

    public GetUserAccountsQueryHandler(IUserAccountRepository userAcountRepository)
    {
        _userAccountsRepository = userAcountRepository;
    }

    public async Task<GetUserAccountsQueryResponse> Handle(GetUserAccountsQuery message, CancellationToken cancellationToken)
    {
        //TODO add validator.
        var userRef = message.UserRef;
        Accounts<Account> accounts;

        if (!string.IsNullOrEmpty(userRef))
        {
            accounts = await _userAccountsRepository.GetAccountsByUserRef(userRef);
        }
        else
        {
            accounts = await _userAccountsRepository.GetAccounts();
        }

        return new GetUserAccountsQueryResponse {Accounts = accounts};
    }
}