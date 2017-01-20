using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountsByHashedId
{
    public class GetEmployerAccountsByHashedIdResponse
    {
        public List<AccountInformation> Accounts { get; set; }
    }
}