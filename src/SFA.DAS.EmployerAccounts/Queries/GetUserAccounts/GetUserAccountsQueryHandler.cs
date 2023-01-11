using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;

public class GetUserAccountsQueryHandler : IAsyncRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>
{
    private readonly IUserAccountRepository _userAccountsRepository;

    public GetUserAccountsQueryHandler(IUserAccountRepository userAcountRepository)
    {
        _userAccountsRepository = userAcountRepository;
    }

    public async Task<GetUserAccountsQueryResponse> Handle(GetUserAccountsQuery message)
    {
        //TODO add validator.
        var userRef = message.UserRef;
        var accounts = new Accounts<Account>();

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