using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityById
{
    public class GetLegalEntityByIdQuery : IAsyncRequest<GetLegalEntityByIdResponse>
    {
        public string HashedAccountId { get; set; }

        public long Id { get; set; }
    }
}