using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetAllEmployerAccounts
{
    public class GetAllEmployerAccountsResponse
    {
        public List<Domain.Models.Account.Account> Accounts { get; set; }
    }
}