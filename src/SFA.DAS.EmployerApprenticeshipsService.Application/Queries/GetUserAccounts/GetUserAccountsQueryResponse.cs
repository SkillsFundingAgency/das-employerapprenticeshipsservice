using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQueryResponse
    {
        public Accounts Accounts { get; set; }
    }
}