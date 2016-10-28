using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesRequest : IAsyncRequest<GetAccountPayeSchemesResponse>
    {
        public string HashedId { get; set; }
        public string ExternalUserId { get; set; }
    }
}