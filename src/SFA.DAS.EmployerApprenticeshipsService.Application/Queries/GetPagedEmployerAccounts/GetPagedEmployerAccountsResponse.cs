using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts
{
    public class GetPagedEmployerAccountsResponse
    {
        public List<Domain.Data.Entities.Account.Account> Accounts { get; set; }
        public int AccountsCount { get; set; }
    }
}