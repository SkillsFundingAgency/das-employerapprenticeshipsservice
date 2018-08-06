using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.Queries.GetAllEmployerAccounts
{
    public class GetAllEmployerAccountsResponse
    {
        public List<Account> Accounts { get; set; }
    }
}