using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services;

public class AccountHandler : IAccountHandler
{
    private readonly IAccountRepository _accountRepository;

    public AccountHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDetailOrganisationsResponse> FindOrganisations(string id)
    {
        var response = new AccountDetailOrganisationsResponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };

        var record = await _accountRepository.Get(id, AccountFieldsSelection.Organisations);

        if (record != null)
        {
            response.StatusCode = SearchResponseCodes.Success;
            response.Account = record;
        }

        return response;
    }

    public async Task<AccountPayeSchemesResponse> FindPayeSchemes(string id)
    {
        var response = new AccountPayeSchemesResponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };

        var record = await _accountRepository.Get(id, AccountFieldsSelection.PayeSchemes);

        if (record != null)
        {
            response.StatusCode = SearchResponseCodes.Success;
            response.Account = record;
        }

        return response;
    }

    public async Task<AccountFinanceResponse> FindFinance(string hashedAccountId)
    {
        var response = new AccountFinanceResponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };

        var getAccountTask = _accountRepository.Get(hashedAccountId, AccountFieldsSelection.Finance);
        var getBalanceTask = _accountRepository.GetAccountBalance(hashedAccountId);

        await Task.WhenAll(getAccountTask, getBalanceTask);

        var account = getAccountTask.Result;
        var balance = getBalanceTask.Result;

        if (account != null)
        {
            response.StatusCode = SearchResponseCodes.Success;
            response.Account = account;
            response.Balance = balance;
        }

        return response;
    }

    public async Task<IEnumerable<AccountSearchModel>> FindAllAccounts(int pagesize, int pageNumber)
    {
        var models = await _accountRepository.FindAllDetails(pagesize, pageNumber);
        return models.Select(x => Map(x));
    }

    public async Task<AccountReponse> Find(string id)
    {
        var response = new AccountReponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };

        var account = await _accountRepository.Get(id, AccountFieldsSelection.None);

        if (account != null)
        {
            response.StatusCode = SearchResponseCodes.Success;
            response.Account = account;
        }

        return response;
    }
    public async Task<AccountReponse> FindTeamMembers(string id)
    {
        var response = new AccountReponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };

        var account = await _accountRepository.Get(id, AccountFieldsSelection.TeamMembers);

        if (account != null)
        {
            response.StatusCode = SearchResponseCodes.Success;
            response.Account = account;
        }

        return response;
    }
    private static AccountSearchModel Map(Core.Models.Account account)
    {
        return new AccountSearchModel
        {
            Account = account.DasAccountName,
            AccountID = account.HashedAccountId,
            PublicAccountID = account.PublicHashedAccountId,
            SearchType = SearchCategory.Account,
            PayeSchemeIds = account.PayeSchemes?.Select(p => p.PayeRefWithOutSlash).ToList()
        };
    }

    public Task<int> TotalAccountRecords(int pagesize)
    {
        return _accountRepository.TotalAccountRecords(pagesize);
    }
}