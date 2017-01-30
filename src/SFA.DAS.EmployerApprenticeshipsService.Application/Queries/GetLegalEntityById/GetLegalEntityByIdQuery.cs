using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityById
{
    public class GetLegalEntityByIdQuery : IAsyncRequest<GetLegalEntityByIdResponse>
    {
        public long Id { get; set; }
    }
}