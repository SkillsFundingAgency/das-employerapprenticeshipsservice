using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts
{
    public class GetPagedEmployerAccountsResponse
    {
        public List<Domain.Models.Account.Account> Accounts { get; set; }
        public int AccountsCount { get; set; }
    }
}