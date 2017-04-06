using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client
{
    public interface IAccountApiClient
    {
        Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null);

        Task<ICollection<TeamMemberViewModel>> GetAccountUsers(string accountId);

        Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userId);

        Task<T> GetResource<T>(string uri) where T : IAccountResource;
    }
}