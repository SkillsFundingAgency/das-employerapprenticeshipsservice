using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange
{
    public class GetPagedEmployerAccountsByDateRangeResponse
    {
        public Accounts<AccountInformation> Accounts { get; set; }
    }
}