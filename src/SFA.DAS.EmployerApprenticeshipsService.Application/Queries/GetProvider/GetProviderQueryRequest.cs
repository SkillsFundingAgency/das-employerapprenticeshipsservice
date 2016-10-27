using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetProvider
{
    public class GetProviderQueryRequest : IAsyncRequest<GetProviderQueryResponse>
    {
        public int ProviderId { get; set; }
    }
}