using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityById
{
    public class GetLegalEntityByIdQuery : IAsyncRequest<GetLegalEntityByIdResponse>
    {
        public long LegalEntityId { get; set; }

        public long AccountId { get; set; }
    }
}