using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransferAllowance
{
    public class GetAccountTransferAllowanceRequest :IAsyncRequest<GetAccountTransferAllowanceResponse>
    {
        public long AccountId { get; set; }
    }
}
