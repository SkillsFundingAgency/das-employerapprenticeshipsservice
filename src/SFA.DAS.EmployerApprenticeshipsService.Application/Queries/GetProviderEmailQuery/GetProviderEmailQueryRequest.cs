using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetProviderEmailQuery
{
    public class GetProviderEmailQueryRequest : IAsyncRequest<GetProviderEmailQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}