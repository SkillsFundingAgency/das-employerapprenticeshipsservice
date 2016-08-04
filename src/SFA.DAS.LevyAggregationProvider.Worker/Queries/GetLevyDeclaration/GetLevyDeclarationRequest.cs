using MediatR;

namespace SFA.DAS.LevyAggregationProvider.Worker.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationRequest : IAsyncRequest<GetLevyDeclarationResponse>
    {
        public long AccountId { get; set; }
    }
}