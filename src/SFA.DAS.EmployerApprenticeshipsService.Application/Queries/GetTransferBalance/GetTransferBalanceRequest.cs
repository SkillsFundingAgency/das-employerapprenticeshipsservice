using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTransferBalance
{
    public class GetTransferBalanaceRequest : IAsyncRequest<GetTransferBalanceResponse>
    {
        public string HashedAccountId { get; set; }
    }
}
