using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetProvider
{
    public class GetProviderQueryRequest : IAsyncRequest<GetProviderQueryResponse>
    {
        public int ProviderId { get; set; }
    }
}