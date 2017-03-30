using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetProvider
{
    public class GetProviderQueryRequest : IAsyncRequest<GetProviderQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}