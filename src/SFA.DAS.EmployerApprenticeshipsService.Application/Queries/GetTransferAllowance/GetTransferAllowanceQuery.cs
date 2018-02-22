using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQuery : IAsyncRequest<GetTransferAllowanceResponse>
    {
        public string HashedAccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
