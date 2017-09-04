using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityById
{
    public class GetLegalEntityByIdQuery : IAsyncRequest<GetLegalEntityByIdResponse>
    {
        public long AccountId { get; set; }

        public long Id { get; set; }
    }
}