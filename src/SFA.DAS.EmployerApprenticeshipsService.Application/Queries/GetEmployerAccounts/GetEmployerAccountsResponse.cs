using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccounts
{
    public class GetEmployerAccountsResponse
    {
        public List<Account> Accounts { get; set; }
    }
}