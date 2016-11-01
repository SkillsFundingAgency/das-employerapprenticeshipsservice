using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationRequest : IAsyncRequest<GetLevyDeclarationResponse>
    {
        public long AccountId { get; set; }
    }
}