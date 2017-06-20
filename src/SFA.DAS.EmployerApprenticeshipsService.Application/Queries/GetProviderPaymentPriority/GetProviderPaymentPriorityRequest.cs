using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority
{
    public class GetProviderPaymentPriorityRequest : IAsyncRequest<GetProviderPaymentPriorityResponse>
    {
        public long AccountId { get; set; }
    }
}