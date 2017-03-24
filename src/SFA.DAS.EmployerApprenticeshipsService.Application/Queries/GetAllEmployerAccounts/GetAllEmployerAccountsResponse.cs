using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetAllEmployerAccounts
{
    public class GetAllEmployerAccountsResponse
    {
        public List<Domain.Data.Entities.Account.Account> Accounts { get; set; }
    }
}