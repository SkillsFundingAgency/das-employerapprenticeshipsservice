using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds
{
    public class GetExpiringAccountFundsQuery : IAsyncRequest<GetExpiringAccountFundsResponse>
    {
        public long AccountId { get; set; }
    }
}
