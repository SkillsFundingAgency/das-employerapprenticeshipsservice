using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntitiy
{
    public class GetAccountLegalEntityRequest : IAsyncRequest<GetAccountLegalEntityResponse>
    {
        public long AccountLegalEntityId { get; set; }
    }
}